using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;
public enum ZombieType { Normal, Speed, Life }
public class Zombie : Destroible
{
    public ZombieType[] priority = new ZombieType[] { ZombieType.Normal, ZombieType.Normal, ZombieType.Normal, ZombieType.Life, ZombieType.Speed };
    public ZombieType zombieType;
    public float zombieBite;
    public float speed = .3f;
    public float up = 1f;
    float seekPath;
    public bool move;
    [LoadPath("scream")]
    public AudioClip[] screamSounds;
    [LoadPath("gib")]
    public AudioClip[] gibSound;
    [LoadPath("Zombie")]
    public AudioClip[] ZombieSound;
    [PathFind("zombieAlive")]
    public GameObject AliveZombie;
    [PathFind("zombieDead")]
    public GameObject DeadZombie;
    public Seeker seeker;
    public float zombieBiteDist = 3;
    Vector3[] pathPoints;
    public Vector3 oldpos;
    public AnimationCurve zombieSpeedCurve;
    public AnimationCurve zombieLifeCurve;

    public override void Init()
    {        
        base.Init();
        seeker = this.GetComponent<Seeker>();
        if (seeker == null) Debug.Log("Could not find seeker");
        velSync = angSync = false;
        posSync = rotSync = true;
    }
    protected override void Awake()
    {
        base.Awake();
        
    }
    protected override void Start()
    {

        if (!_Loader.disablePathFinding) seeker.enabled = true;
        _Game.zombies.Add(this);
        base.Start();

    }

    public void CreateZombie(int stage)
    {
        var speed = Random.Range(zombieSpeedCurve.Evaluate(stage), zombieSpeedCurve.Evaluate(stage + 3));
        var life = Random.Range(zombieLifeCurve.Evaluate(stage), zombieLifeCurve.Evaluate(stage + 10));
        if (zombieType == ZombieType.Life) life *= 3;
        if (zombieType == ZombieType.Speed) { speed *= 1.5f; life *= .7f; }
        RPCSetup(speed, life, (int)priority.Random());
    }

    public void RPCSetup(float zombiespeed, float zombieLife, int priority) { CallRPC("Setup", zombiespeed, zombieLife, priority); }
    [RPC]
    public void Setup(float zombiespeed, float zombieLife, int priority)
    {
        Alive = true;
        Sync = true;
        ResetSpawn();
        gameObject.layer = LayerMask.NameToLayer("Enemy");
        _TimerA.AddMethod(UnityEngine.Random.Range(0, 1000), PlayRandom);
        AliveZombie.renderer.enabled = true;
        DeadZombie.renderer.enabled = false;
        zombieType = (ZombieType)priority;
        speed = zombiespeed;
        transform.localScale = Vector3.one * Mathf.Max(zombieLife / 100f, 1f);
        Life = (int)zombieLife;
        
    }
    [RPC]
    public override void Die(int killedby)
    {
        if (!Alive) { return; }        
        Sync = false;
        Alive = false;
        gameObject.layer = LayerMask.NameToLayer("HitLevelOnly");        
        PlayRandSound(gibSound);
        AliveZombie.renderer.enabled = false;
        DeadZombie.renderer.enabled = true;

        if (isController)
        {
            foreach (Player p in players)
                if (p != null && p.OwnerID == killedby)
                    p.AddFrags(+1, 1);
        }
    }
    public float tiltTm;
    public float spawninTM = 5;
    protected override void Update()
    {
        
        base.Update();
        if (!Alive || selected == -1) return;
        zombieBite += Time.deltaTime;
        var ipl = Nearest();
        if (ipl != null)
        {
            Vector3 pathPointDir;
            Vector3 zToPlDir = ipl.transform.position - pos;
            if (zToPlDir.magnitude > zombieBiteDist)
            {
                pathPointDir = (zToPlDir.magnitude < 10 && Mathf.Abs(zToPlDir.y) < 1) ? zToPlDir : (GetRay(ipl)?? GetPlayerPathPoint(ipl) ?? GetNextPathPoint(ipl) ?? zToPlDir);
                Debug.DrawLine(pos, pos + pathPointDir);
                pathPointDir.y = 0;
                rot = Quaternion.LookRotation(pathPointDir.normalized);
                move = true;
                tiltTm += Time.deltaTime;
                if (tiltTm > spawninTM && isController)
                {
                    tiltTm = 0;
                    if (Vector3.Distance(oldpos, pos) / spawninTM < .5f)
                        ResetSpawn();                        
                    oldpos = pos;
                }
            }
            else
            {
                move = false;
                tiltTm = 0;
                if (zombieBite > 1)
                {
                    zombieBite = 0;
                    PlayRandSound(screamSounds);
                    if ((build || ipl is Tower) && isController) ipl.RPCSetLife(ipl.Life - 10 - _Game.stage, -1);
                }
            }
        }
        else
        {
            move = false;
            tiltTm = 0;
        }        
    }

    private Vector3? GetRay(Destroible ipl)
    {
        var r = new Ray(pos, ipl.pos - pos);
        if (Math.Abs(ipl.posy - posy) > 2) return null;
        if (Physics.Raycast(r, Vector3.Distance(ipl.pos, pos), 1 << LayerMask.NameToLayer("Level")))
            return null;
        return ipl.pos - pos;
    }
    private Destroible Nearest()
    {

        Destroible pl =
            _Game.towers.Where(a => a != null && !a.disableZombieAtack && Vector3.Distance(a.pos, pos) < 10).Cast<Destroible>().Union(players).Where(a => a != null && a.Alive).OrderBy(a => Vector3.Distance(a.pos, pos)).FirstOrDefault();
        return pl;
    }
    void FixedUpdate()
    {
        if (move && Alive)
        {
            if (rigidbody.velocity.magnitude < 10)
            {
                Vector3 v = rigidbody.velocity;
                v.x = v.z = 0;
                rigidbody.velocity = v;
            }
            pos += rot * new Vector3(0, 0, speed * Time.fixedDeltaTime * Time.timeScale);
        }
        
    }
    private Vector3? GetPlayerPathPoint(Destroible ipl)
    {
        if (ipl is Player)
        {
            var pathPoints = ((Player)ipl).plPathPoints;
            return FindNextPoint(pathPoints);
        }
        return null;
    }
    private Vector3? FindNextPoint(IList<Vector3> points)
    {
        if (points == null || points.Count == 0) return null;
        Vector3 nearest = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        bool found = false;
        int ni = 0;
        for (int i = 0; i < points.Count; i++)
        {
            //if (i != 0) //Debug.DrawLine(points[i - 1], points[i]);
            Vector3 newp = points[i] - pos;
            if (newp.magnitude < nearest.magnitude)
            {
                nearest = newp;
                ni = i;
                if (nearest.magnitude < 6)
                    found = true;
            }
        }
        if (found)
            while(true)
            {
                ni++;
                if (ni >= points.Count) break;
                if (Vector3.Distance(points[ni], pos) > 3)
                    return points[ni] - pos;
            }

        return null;
    }
    private Vector3? GetNextPathPoint(Destroible ipl)
    {
        if (_Loader.disablePathFinding) return null;
        if ((seekPath -= Time.deltaTime) < 0)
        {
            seeker.StartPath(this.transform.position, ipl.transform.position);
            seekPath = UnityEngine.Random.Range(.5f, 1);
        }

        return FindNextPoint(pathPoints);
        
    }
    void PathComplete(Vector3[] points)
    {
        pathPoints = points;
    }
    private void PlayRandom()
    {
        if (this != null && Alive)
        {
            _TimerA.AddMethod(UnityEngine.Random.Range(5000, 50000), PlayRandom);            
            PlayRandSound(ZombieSound);
            
        }
    }
    public override void ResetSpawn()
    {
        spawninTM = Random.Range(1, 10); 
        GameObject[] gs = GameObject.FindGameObjectsWithTag("SpawnZombie");
        
        Destroible pl = Nearest();
        if (pl == null)
        {
            pos = gs.First().transform.position;
        }
        else
        {
            //var neargs  = gs.Where(a => Vector3.Distance(a.transform.position, pl.pos) < 100 && Math.Abs(a.transform.position.y - pl.pos.y) < 3).ToList();
            var b = gs.Where(a => a.GetComponent<MeshFilter>().collider.bounds.Contains(pl.pos)).Random();
            var o = gs.OrderBy(a => Vector3.Distance(a.transform.position, pl.pos));
            pos = (b ?? o.FirstOrDefault(a => Math.Abs(a.transform.position.y - pl.pos.y) < 3) ?? o.First()
                ).transform.position;
        }
        rot = Quaternion.identity;
        rigidbody.velocity = Vector3.zero;
    }
    
    public override void OnPlayerConnected1(NetworkPlayer np)
    {
        base.OnPlayerConnected1(np);
        RPCSetup((float)speed, (float)Life, (int)zombieType);
        if(!Alive) RPCDie(-1);
    }
}
