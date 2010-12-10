using System.Linq;
using UnityEngine;


using System.Collections;
using doru;
using System.Collections.Generic;
using System;
public enum Team : int { Red, Blue, None }
[Serializable]
public class Player : IPlayer
{
    public new bool dead { get { return !Alive && spawned; } }
    public new Team team = Team.None;
    public float force;
    public int score;
    public float freezedt;
    public int guni;
    public int fps;
    public int ping;
    public int deaths;
    new public string nick;
    public bool spawned;
    public int frags;
    [PathFind("speedparticles")]
    public ParticleEmitter speedparticles;
    const int life = 100;
    [PathFind("Guns")]
    public Transform guntr;
    [GenerateEnums("GunType")]
    public List<GunBase> guns = new List<GunBase>();
    public int selectedgun;
    public GunBase gun { get { return guns[selectedgun]; } }
    public float defmass;
    [PathFind("Sphere")]
    public GameObject model;
    public override void Init()
    {
        base.Init();
        guns = guntr.GetChild(0).GetComponentsInChildren<GunBase>().ToList();
        shared = false;        
    }
    protected override void Awake()
    {
        AliveMaterial = model.renderer.material;        
        Debug.Log("player awake");
        if (!build)
            score = 10000;
        defmass = rigidbody.mass;
        this.rigidbody.maxAngularVelocity = 3000;
        if (networkView.isMine)
        {
            _Game._localiplayer = _Game._localPlayer = this;
            RPCSetOwner();
            RPCSetUserInfo(LocalUserV.nick);
            RPCSpawn();
        }
        //speedparticles = transform.Find("speedparticles").GetComponent<ParticleEmitter>();

        base.Awake();
    }
    protected override void Start()
    {
        base.Start();
    }
    public override void OnPlayerConnected1(NetworkPlayer np)
    {
        base.OnPlayerConnected1(np);
        networkView.RPC("RPCSetUserInfo", np, nick);
        networkView.RPC("RPCSetFrags", np, frags, score);
        networkView.RPC("RPCSetDeaths", np, deaths);
        networkView.RPC("RPCSetTeam", np, (int)team);
        networkView.RPC("RPCSpawn", np);        
        networkView.RPC("RPCAlive", np, Alive);
        networkView.RPC("RPCSelectGun", np, selectedgun);
        //if (spawned && dead) networkView.RPC("RPCDie", np, -1);
    }
    public override void OnSetOwner()
    {
        print("set owner" + OwnerID);
        if (isOwner)
            tag = name = "LocalPlayer";
        else
            name = "RemotePlayer" + OwnerID;
        _Game.players[OwnerID] = this;
        _Game.WriteMessage(nick + " законектился");
    }
    [RPC]
    public void RPCSpawn()
    {        
        print(pr + "+" + OwnerID);
        CallRPC();
        if (isOwner)
        {
            RPCSetTeam((int)team);
            RPCAlive(Alive);
            transform.position = SpawnPoint();
            transform.rotation = Quaternion.identity;
        }        
    }

    public override Vector3 SpawnPoint()
    {
        //GameObject[] gs = GameObject.FindGameObjectsWithTag("Spawn" + team.ToString());
        //return gs.OrderBy(a => Vector3.Distance(a.transform.position, transform.position)).First().transform.position;
        
        return GameObject.FindGameObjectWithTag("Spawn" + team.ToString()).transform.position;
    }
    public void SelectGun(int id)
    {
        if (guns.Count(a => a.group == id && a.patronsLeft > 0) == 0) return;
        bool foundfirst = false;
        bool foundnext = false;
        for (int i = selectedgun; i < guns.Count; i++)
            if (guns[i].group == id && guns[i].patronsLeft > 0)
            {
                if (foundfirst) { selectedgun = i; foundnext = true; break; }
                foundfirst = true;
            }
        if (!foundnext)
            for (int i = 0; i < guns.Count; i++)
                if (guns[i].group == id && guns[i].patronsLeft > 0)
                {
                    selectedgun = i;
                    break;
                }
        
        RPCSelectGun(selectedgun);
    }
    [LoadPath("change")]
    public AudioClip changeSound;
    public static Transform Root(string tag,Transform t2)
    {
        Transform t = t2;
        while(t.parent != null && t.parent.tag != tag)
            t = t.parent;

        return t;
    }
    [RPC]
    public void RPCSelectGun(int i)
    {
        CallRPC(i);
        PlaySound(changeSound);
        selectedgun = i;
        foreach (GunBase gb in guns)
            gb.DisableGun();

        if(Alive)
            guns[selectedgun].EnableGun();
    }
    protected override void Update()
    {
        if (_TimerA.TimeElapsed(100))
        {
            if (plPathPoints.Count == 0 || Vector3.Distance(pos, plPathPoints.Last()) > 1)
            {
                plPathPoints.Add(pos);
                if (plPathPoints.Count > 10) plPathPoints.RemoveAt(0);
            }
        }
        
        if (DebugKey(KeyCode.Keypad1))
            RPCSetLife(-1, -1);
        multikilltime -= Time.deltaTime;
        if (this.rigidbody.velocity.magnitude > 30)
        {
            speedparticles.worldVelocity = this.rigidbody.velocity / 10;
            if (_TimerA.TimeElapsed(100))
            {
                speedparticles.transform.rotation = Quaternion.identity;
                speedparticles.Emit();
            }
        }
        if (freezedt >= 0)
            freezedt -= Time.deltaTime * 5;
        if (isOwner && lockCursor)
        {
            //NextGun(Input.GetAxis("Mouse ScrollWheel"));
            SelectGun();
            if (Input.GetKey(KeyCode.LeftShift))
                this.transform.rotation = Quaternion.identity;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (nitro > 10 || !build)
                {
                    nitro -= 10;
                    RCPJump();
                }
            }
        }
        //UpdateAim();
        base.Update();
    }

    private void SelectGun()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SelectGun(1);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            SelectGun(2);
        if (Input.GetKeyDown(KeyCode.Alpha3))
            SelectGun(3);
        if (Input.GetKeyDown(KeyCode.Alpha4))
            SelectGun(4);
        if (Input.GetKeyDown(KeyCode.Alpha5))
            SelectGun(5);
        if (Input.GetKeyDown(KeyCode.Alpha6))
            SelectGun(6);
        if (Input.GetKeyDown(KeyCode.Alpha7))
            SelectGun(7);
        if (Input.GetKeyDown(KeyCode.Alpha8))
            SelectGun(8);
        if (Input.GetKeyDown(KeyCode.Alpha9))
            SelectGun(9);
    }
    public void UpdateAim()
    {
        if (isOwner) syncRot = _Cam.transform.rotation;
        guntr.rotation = syncRot;
    }
    
    protected virtual void FixedUpdate()
    {
        if (isOwner) FixedLocalMove();
        //UpdateAim();
    }
    private void FixedLocalMove()
    {
        if (lockCursor)
        {
            Vector3 moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            moveDirection = _Cam.transform.TransformDirection(moveDirection);
            moveDirection.y = 0;
            moveDirection.Normalize();


            Vector3 v = this.rigidbody.velocity;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                this.rigidbody.angularVelocity = Vector3.zero;
                this.rigidbody.AddForce(moveDirection * Time.fixedDeltaTime * force * 7);
                v.x *= .65f;
                v.z *= .65f;
                this.rigidbody.velocity = v;
            }
            else
            {
                this.rigidbody.AddTorque(new Vector3(moveDirection.z, 0, -moveDirection.x) * Time.fixedDeltaTime * 300);                
            }
        }
    }
    [RPC]
    private void RCPJump()
    {
        CallRPC();
        transform.rigidbody.MovePosition(rigidbody.position + new Vector3(0, 1, 0));
        rigidbody.AddForce(_Cam.transform.rotation * new Vector3(0, 0, 1000));
        PlaySound(nitrojumpSound);
    }
    [LoadPath("nitrojump")]
    public AudioClip nitrojumpSound;
    public void NextGun(float a)
    {
        if (a != 0)
        {
            if (a > 0)
                guni++;
            if (a < 0)
                guni--;
            if (guni > guns.Count - 1) guni = 0;
            if (guni < 0) guni = guns.Count - 1;
            RPCSelectGun(guni);
        }
    }


    [RPC]
    public void RPCSetTeam(int t)
    {
        print(pr);
        CallRPC(t);
        team = (Team)t;
    }
    [RPC]
    public void RPCSetDeaths(int d) { deaths = d; }

    void OnCollisionEnter(Collision collisionInfo)
    {
        if (!Alive) return;
        if (isOwner && collisionInfo.relativeVelocity.y > 30)
            RPCPowerExp(this.transform.position);

        Box b = collisionInfo.gameObject.GetComponent<Box>();
        if (b != null && isOwner && b.OwnerID != -1 && (b.isOwner || players[b.OwnerID].team != team || mapSettings.DM) &&
            !(b is Player) && !(b is Zombie) &&
            collisionInfo.rigidbody.velocity.magnitude > 20
            )
        {
            RPCSetLife(Life - (int)collisionInfo.rigidbody.velocity.magnitude * 2, b.OwnerID);
        }
    }
    [LoadPath("powerexp")]
    public AudioClip powerexpSound;
    [LoadPath("wave")]
    public GameObject WavePrefab;
    [RPC]
    private void RPCPowerExp(Vector3 v)
    {
        CallRPC(v);
        PlaySound(powerexpSound, 4);
        GameObject g = (GameObject)Instantiate(WavePrefab, v, Quaternion.Euler(90, 0, 0));
        Explosion e = g.AddComponent<Explosion>();
        e.OwnerID = OwnerID;
        e.self = this;
        e.exp = 3000;
        e.radius = 8;
        e.damage = 200;
        if(isOwner)
            _Cam.exp = 2;
        Destroy(g, 1.6f);
    }


    [RPC]
    public override void RPCSetLife(int NwLife, int killedby)
    {
        if (!Alive) return;
        CallRPC(NwLife, killedby);
        if (isOwner)
            _GameWindow.Hit(Mathf.Abs(Life - NwLife) * 2);



        if (isEnemy(killedby) || NwLife > Life)
            Life = Math.Min(NwLife, 100);


        if (Life <= 0 && isOwner)
            RPCDie(killedby);


    }


    [RPC]
    private void RPCSetUserInfo(string nick)
    {
        CallRPC(nick);
        this.nick = nick;
    }
    [LoadPath("Standard Assets/Detonator/Prefab Examples/Detonator-Chunks")]
    public GameObject detonator;
    [RPC]
    public override void RPCDie(int killedby)
    {
        
        if (!Alive) return;
        print(pr);
        CallRPC(killedby);
        Instantiate(detonator, transform.position, Quaternion.identity);
        deaths++;
        if (isOwner)
        {
            if (!mapSettings.zombi) _TimerA.AddMethod(10000, delegate { RPCAlive(true); });
            foreach (Player p in GameObject.FindObjectsOfType(typeof(Player)))
            {
                if (p.OwnerID == killedby)
                {
                    if (p.isOwner)
                    {
                        _Game.RPCWriteMessage(_localPlayer.nick + " Óìåð ñàì ");
                        _localPlayer.SetFrags(-1, -5);
                    }
                    else if (p.team != _localPlayer.team || mapSettings.DM)
                    {
                        _Game.RPCWriteMessage(p.nick + " Óáèë " + _localPlayer.nick);
                        p.SetFrags(+1, 20);
                    }
                    else
                    {
                        _Game.RPCWriteMessage(p.nick + " Óáèë ñîþçíèêà " + _localPlayer.nick);
                        p.SetFrags(-1, -10);
                    }
                }
            }
            if (killedby == -1)
            {
                _Game.RPCWriteMessage(_localPlayer.nick + " Ïîãèá ");
                _localPlayer.SetFrags(-1, -5);
            }
            lockCursor = false;
            RPCAlive(false);
        }
    }
    public Material AliveMaterial;
    public Material deadMaterial;

    
    [RPC]
    public void RPCAlive(bool value)
    {
        CallRPC(value);
        foreach (var t in GetComponentsInChildren<Transform>())
            t.gameObject.layer = value ? LayerMask.NameToLayer("Default") : LayerMask.NameToLayer("HitLevelOnly");

        Alive = value;
        if(value)
            spawned = true;
        model.renderer.material = value? AliveMaterial:deadMaterial;                
        foreach (GunBase gunBase in guns.Concat(guns))
            gunBase.Reset();
        if (isOwner)
            SelectGun(1);
        Life = life;
        freezedt = 0;

    }

    float multikilltime;
    int multikill;
    public void SetFrags(int i, int sc)
    {
        RPCSetFrags(frags + i, score + sc);
    }
    [LoadPath("toasty")]
    public AudioClip[] multikillSounds;
    [RPC]
    public void RPCSetFrags(int i, int sc)
    {
        CallRPC(i, sc);
        if (isOwner)
        {
            if (multikilltime > 0)
                multikill++;
            else
                multikill = 0;
            multikilltime = 3;


            if (multikill >= 1)
            {
                PlayRandSound(multikillSounds, 4);
                if (isOwner)
                {
                    _Cam.ScoreText.text = "x" + (multikill + 1);
                    _Cam.ScoreText.animation.Play();
                }
            }
        }




        frags = i;
        score = sc;



    }
    public static Vector3 Clamp(Vector3 velocityChange, float maxVelocityChange)
    {


        velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
        velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
        velocityChange.y = Mathf.Clamp(velocityChange.y, -maxVelocityChange, maxVelocityChange);
        return velocityChange;
    }

    [RPC]
    public void RPCCarIn()
    {
        CallRPC();
        Show(false);
    }
    public override Quaternion rot
    {
        get
        {
            return guntr.rotation;
        }
        set
        {
            guntr.rotation = value;
        }
    }
}