﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.3053
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Chatbox2 {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "2.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Res {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Res() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Chatbox2.Res", typeof(Res).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Такой ник уже зарегестрирован!.
        /// </summary>
        internal static string alreadyregistered {
            get {
                return ResourceManager.GetString("alreadyregistered", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to сайт проекта: http://cslive.mindswitch.ru/
        ///правила: разговаривать только о cslive остальные темы запрешенны, личные сообшения неотсылать!
        ///команды:
        ////unregister выйти из чата
        ////ping пинг
        ////register  [nick] зарегестрировать ник
        ////whois посмотреть список юзеров
        ////setinfo [text] написать инфо о себе
        ////ban [nick] [minutes] забанить юзера на указанное время.
        /// </summary>
        internal static string help {
            get {
                return ResourceManager.GetString("help", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to HTTP/1.1 404 Not Found
        ///Connection: Keep-Alive
        ///Content-Type: text/plain
        ///Content-Length: 9
        ///
        ///not found.
        /// </summary>
        internal static string httpnotfound {
            get {
                return ResourceManager.GetString("httpnotfound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to HTTP/1.1 200 OK
        ///Connection: Keep-Alive
        ///Content-Length: {0}
        ///Content-Type: text/HTML; charset=windows-1251
        ///
        ///{1}.
        /// </summary>
        internal static string httpsend {
            get {
                return ResourceManager.GetString("httpsend", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to На етом icq номере лимит регистраций превышен,Пожалуйста зарегестрируйтесь по icq номеру.
        /// </summary>
        internal static string limitreached {
            get {
                return ResourceManager.GetString("limitreached", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Сервер перегружен попробуйте зарегестрироватся завтра.
        /// </summary>
        internal static string nouins {
            get {
                return ResourceManager.GetString("nouins", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Для того чтобы зарегистрировать свой ник, напишите: /register имя.
        /// </summary>
        internal static string register {
            get {
                return ResourceManager.GetString("register", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ^.?register ([\x00\x21\x22\x23\x24\x25\x26\x27\x28\x29\x2A\x2B\x2C\x2D\x2E\x2F\x3A\x3B\x3C\x3D\x3E\x3F\x5B\x5C\x5D\x5E\x5F\x7B\x7C\x7D\x7E\xA9\xAE\w ]+).
        /// </summary>
        internal static string registerMatch {
            get {
                return ResourceManager.GetString("registerMatch", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ^.?setinfo (.+).
        /// </summary>
        internal static string setinfoMatch {
            get {
                return ResourceManager.GetString("setinfoMatch", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to неизвестная команда, смотрите /help.
        /// </summary>
        internal static string unknowncommand {
            get {
                return ResourceManager.GetString("unknowncommand", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to \A/.*(\r?\n?.?)*\z.
        /// </summary>
        internal static string unknowncommandMatch {
            get {
                return ResourceManager.GetString("unknowncommandMatch", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Вы вышли из чата.
        /// </summary>
        internal static string unregister {
            get {
                return ResourceManager.GetString("unregister", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ^.?unregister$.
        /// </summary>
        internal static string unregisterMatch {
            get {
                return ResourceManager.GetString("unregisterMatch", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Добро пожаловать в чат CSLIVE (cslive.mindswitch.ru)
        ///Чтобы воити в чат напишите /register
        ///Чтобы посмотреть список команд напишите /help
        ///если будут проблемы с чатом то обратитесь по номеру 1231925.
        /// </summary>
        internal static string welcome {
            get {
                return ResourceManager.GetString("welcome", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Вы зарегестрированы! Теперь можете посылать сообшения в чат.
        /// </summary>
        internal static string youareregistered {
            get {
                return ResourceManager.GetString("youareregistered", resourceCulture);
            }
        }
    }
}
