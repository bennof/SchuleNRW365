using System;

namespace Templates {
    public class INDEX :Pink.Template {
        public override void Render(Pink.Request req, object o){
            req.WriteString( "<!DOCTYPE html>\r\n<html lang=\"en\">\r\n<head>\r\n\r\n    <meta charset=\"utf-8\">\r\n    <tit" +
    "le>Benjamin Benno Falkner</title> \r\n    <meta name=\"description\" content=\"my per" +
    "sonal github index page\">\r\n    <meta name=\"author\" content=\"Benjamin Benno Falkn" +
    "er\">\r\n    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">\r\n" +
    "    \r\n    <link href=\"https://fonts.googleapis.com/css?family=Roboto:400,300,600" +
    "\" rel=\"stylesheet\" type=\"text/css\">\r\n    <link rel=\"stylesheet\" href=\"https://cd" +
    "njs.cloudflare.com/ajax/libs/font-awesome/4.7.0/css/font-awesome.min.css\">\r\n    " +
    "<link rel=\"stylesheet\" href=\"css/tabularrasa.css\"> \r\n    <script src=\"js/recall." +
    "js\"></script>\r\n    <script src=\"js/edu365.js\"></script>\r\n    <script src=\"js/_.j" +
    "s\"></script>\r\n\r\n    <!-- Script -->\r\n    <script>\r\n        function Print (resul" +
    "t){document.getElementById(\'data\').innerHTML += JSON.stringify(result) + \'<br>\';" +
    "}\r\n        function PrintOBJ (result){\r\n            if(ReCall.IsString(result)){" +
    "\r\n                document.getElementById(\'data\').innerHTML += JSON.stringify(re" +
    "sult) + \'<br>\';\r\n            } else {\r\n                var str = \"JSON: <br>\";\r\n" +
    "                for(var key in result){\r\n                    if (Array.isArray(r" +
    "esult[key])){\r\n                        str += \">>> \"+key+\": ... <br>\";\r\n        " +
    "                var a = result[key];\r\n                        for(i in a) {\r\n   " +
    "                         if(ReCall.IsString(a[i])){ \r\n                          " +
    "      str += \">>> >>> \"+a[i]+\"<br>\";\r\n                            } else {\r\n    " +
    "                            str += \">>> >>> \"+JSON.stringify(a[i])+\"<br>\";\r\n    " +
    "                        }\r\n                        }\r\n                    } else" +
    " {\r\n                        str += \">>> \"+key+\": \"+JSON.stringify(result[key])+\"" +
    "<br>\";\r\n                    }\r\n                }\r\n                document.getEl" +
    "ementById(\'data\').innerHTML += str + \'<br>\';\r\n            }\r\n        }\r\n        " +
    "function PError (result){document.getElementById(\'data\').innerHTML += \'<strong s" +
    "tyle=\"color: red\">ERROR: \'+result+\'</strong><br>\';}\r\n        function getuser(gi" +
    "venname, surname){Edu365.User.Get(givenname, surname,Print,Error);}\r\n        fun" +
    "ction getteam(name){Edu365.Team.Get(name,Print,PError);}\r\n        function newte" +
    "am(name){Edu365.Team.Create(name,Print,PError);}\r\n        function rmteam(teamna" +
    "me){Edu365.Team.Get(teamname,function(team){Edu365.Team.Delete(team,Print,PError" +
    ");},PError);}\r\n        function getmember(teamname){Edu365.Team.Get(teamname,fun" +
    "ction(team){Edu365.Team.GetMember(team,Print,PError);},PError);}\r\n        functi" +
    "on getowner(teamname){Edu365.Team.Get(teamname,function(team){Edu365.Team.GetOwn" +
    "er(team,Print,PError);},PError);}\r\n        function addmember(teamname,fusername" +
    ",lusername){Edu365.Team.Get(teamname,function(team){Edu365.User.Get(fusername,lu" +
    "sername,function(user){Edu365.Team.AddMember(team,user,Print,PError);},PError);}" +
    ",PError);}\r\n        function addowner(teamname,fusername,lusername){Edu365.Team." +
    "Get(teamname,function(team){Edu365.User.Get(fusername,lusername,function(user){E" +
    "du365.Team.AddOwner(team,user,Print,PError);},PError);},PError);}\r\n        funct" +
    "ion rmmember(teamname,fusername,lusername){Edu365.Team.Get(teamname,function(tea" +
    "m){Edu365.User.Get(fusername,lusername,function(user){Edu365.Team.RmMember(team," +
    "user,Print,PError);},PError);},PError);}\r\n        function rmowner(teamname,fuse" +
    "rname,lusername){Edu365.Team.Get(teamname,function(team){Edu365.User.Get(fuserna" +
    "me,lusername,function(user){Edu365.Team.RmOwner(team,user,Print,PError);},PError" +
    ");},PError);}\r\n        \r\n        function call(method,url,body){\r\n            if" +
    "(method==\'GET\') {\r\n                Edu365.Get(url,PrintOBJ,PError);\r\n           " +
    " } else {\r\n                Edu365.Set(method,url,body,PrintOBJ,PError);\r\n       " +
    "     }\r\n        }\r\n\r\n\r\n        function checkstudents(){\r\n            ReCall.AJA" +
    "X.Request(\"/schild/students.json\",{\r\n                onSuccess: function(result)" +
    "{\r\n                    Edu365.Students.Check(JSON.parse(result),Print,PError);\r\n" +
    "                }, onError: PError});\r\n        }\r\n\r\n        function checkteache" +
    "rs(){\r\n            ReCall.AJAX.Request(\"/schild/faculty.json\",{\r\n               " +
    " onSuccess:function(result){\r\n                    Edu365.Faculty.Check(JSON.pars" +
    "e(result),Print,PError);\r\n                }, onError: PError})\r\n        }\r\n\r\n   " +
    "     function checkcourses(){\r\n            ReCall.AJAX.Request(\"/schild/courses." +
    "json\",{\r\n                onSuccess:function(result){\r\n                    Edu365" +
    ".Faculty.CheckTeams(JSON.parse(result),coursenaming,Print,PError);\r\n            " +
    "    }, onError: PError})\r\n        }\r\n\r\n        function checkcoursess2(){\r\n     " +
    "       ReCall.AJAX.Request(\"/schild/s2/courses.json\",{\r\n                onSucces" +
    "s:function(result){\r\n                    Edu365.Faculty.CheckTeams(JSON.parse(re" +
    "sult),coursenamings2,Print,PError);\r\n                }, onError: PError})\r\n     " +
    "   }\r\n\r\n        function mkclasses(){\r\n            ReCall.AJAX.Request(\"/schild/" +
    "classes.json\",{\r\n                onSuccess:function(result){\r\n                  " +
    "  Edu365.Faculty.CreateTeam(JSON.parse(result),function(elem){return \"Klasse \"+e" +
    "lem.class;},Print,PError);\r\n                }, onError: PError})\r\n        }\r\n\r\n " +
    "       function fillclasses(){\r\n            ReCall.AJAX.Request(\"/schild/student" +
    "s.json\",{\r\n                onSuccess:function(result){\r\n                    Edu3" +
    "65.Students.Assign(JSON.parse(result),function(elem){return \"Klasse \"+elem.class" +
    ";},Print,PError);\r\n                }, onError: PError})\r\n        }\r\n\r\n        \r\n" +
    "\r\n        function coursenaming(elem){\r\n            var name = elem.subject;\r\n  " +
    "          if (elem.subject_type == \"PUK\" && elem.subject != \"VF\" && elem.subject" +
    " != \"KL\"){\r\n                return name+\" \"+elem.class+\" \"+elem.teacher;\r\n      " +
    "      } else if (elem.subject_type == \"PUT\"){\r\n                if(elem.grade==\"0" +
    "7\" && (elem.subject == \"IF\" || elem.subject==\"ATX\")){\r\n                    retur" +
    "n;\r\n                }\r\n                return name+\" \"+elem.grade+\" \"+elem.teach" +
    "er;\r\n            } else if (elem.subject_type == \"WPI\"){\r\n                return" +
    " \"WP-\"+name+\" \"+elem.grade+\" \"+elem.teacher;\r\n            //} else if (elem.subj" +
    "ect_type == \"GKM\" || elem.subject_type == \"GKS\"){\r\n            //    return name" +
    "+\" \"+elem.grade+\" \"+elem.teacher;\r\n            } else if (elem.subject_type == \"" +
    "EGSN\"){\r\n                return name+\" \"+elem.grade+\" \"+elem.teacher;\r\n         " +
    "   } else if (elem.subject_type == \"G\"){\r\n                if(elem.subject == \"PH" +
    "\"){\r\n                    return name+\" \"+elem.grade+\" \"+elem.teacher;\r\n         " +
    "       } else {\r\n                    return name+\" \"+elem.grade+\" GK \"+elem.teac" +
    "her;\r\n                }\r\n            } else if (elem.subject_type == \"E\"){\r\n    " +
    "            if(elem.subject == \"PH\"){\r\n                    return name+\" \"+elem." +
    "grade+\" \"+elem.teacher;\r\n                } else {\r\n                    return na" +
    "me+\" \"+elem.grade+\" EK \"+elem.teacher;\r\n                }\r\n            }\r\n\r\n    " +
    "        //return \">>>>> \"+name+\" \"+elem.teacher+\" \"+elem.class+\" \"+elem.grade+\" " +
    "\"+elem.subject_type;\r\n        }\r\n\r\n        function coursenamings2(elem){\r\n     " +
    "       if (elem.subject.startsWith(\"VX\")) {return ;}\r\n            return elem.su" +
    "bject +\" \"+elem.grade+\" \"+elem.teacher;\r\n        }\r\n\r\n        function mkcourses" +
    "(){\r\n            ReCall.AJAX.Request(\"/schild/courses.json\",{\r\n                o" +
    "nSuccess:function(result){\r\n                    Edu365.Faculty.CreateTeam(JSON.p" +
    "arse(result),coursenaming,Print,PError);\r\n                    //testloop(JSON.pa" +
    "rse(result),coursenaming,Print,PError);\r\n                }, onError: PError})\r\n " +
    "       }\r\n\r\n        function mkcoursess2(){\r\n            ReCall.AJAX.Request(\"/s" +
    "child/s2/courses.json\",{\r\n                onSuccess:function(result){\r\n         " +
    "           Edu365.Faculty.CreateTeam(JSON.parse(result),coursenamings2,Print,PEr" +
    "ror);\r\n                    //testloop(JSON.parse(result),coursenamings2,Print,PE" +
    "rror);\r\n                }, onError: PError})\r\n        }\r\n\r\n        function fill" +
    "courses(start){\r\n            ReCall.AJAX.Request(\"/schild/allocation.json?start=" +
    "\"+start,{\r\n                onSuccess:function(result){\r\n                    Edu3" +
    "65.Students.Assign(JSON.parse(result),coursenaming,Print,PError);\r\n             " +
    "   }, onError: PError})\r\n        }\r\n\r\n        function fillcoursess2(){\r\n       " +
    "     ReCall.AJAX.Request(\"/schild/s2/allocation.json\",{\r\n                onSucce" +
    "ss:function(result){\r\n                    Edu365.Students.Assign(JSON.parse(resu" +
    "lt),coursenamings2,Print,PError);\r\n                }, onError: PError})\r\n       " +
    " }\r\n\r\n\r\n\r\n\r\n\r\n\r\n        function testloop(data,func,print,err){\r\n            var" +
    " elem = data.shift();\r\n            if(elem == null || ReCall.IsEmpty(elem)) {pri" +
    "nt(\'done\');return;}\r\n            var name = func(elem); \r\n            if (name)\r" +
    "\n                print(name);\r\n            testloop(data,func,print,err)\r\n      " +
    "  }\r\n\r\n\r\n\r\n        ReCall.Ready(function(){\r\n            //init Edu365\r\n        " +
    "    Edu365.Init({\r\n                URL: \'https://login.microsoftonline.com/26f74" +
    "32b-b6b6-4291-adc0-090590f363cf/oauth2/v2.0/authorize\',\r\n                Config:" +
    " _js.Config,\r\n                onNone: Edu365.Login,\r\n                onLogin: fu" +
    "nction(){\r\n                    document.getElementById(\'username\').innerHTML = E" +
    "du365.Username(); \r\n                    document.getElementById(\'data\').innerHTM" +
    "L = \">>> Login: \"+ Edu365.Username(); \r\n                    //Edu365.Photo(funct" +
    "ion(result){\r\n                    //    document.getElementById(\"userimage\").set" +
    "Attribute(\"src\", result);\r\n                    //});\r\n                }\r\n       " +
    "     });\r\n\r\n            document.getElementById(\"input\").addEventListener(\"keyup" +
    "\", function(event) {\r\n                event.preventDefault();\r\n                i" +
    "f (event.keyCode === 13) {\r\n                    var input = document.getElementB" +
    "yId(\'input\').value;\r\n                    document.getElementById(\'data\').innerHT" +
    "ML += \'<p> ::: Edo365 \'+input.trim()+\'</p>\';\r\n                    try {\r\n       " +
    "                 eval(input.trim());\r\n                    } catch(err) {PError(e" +
    "rr.message);}\r\n                }\r\n            });\r\n        });\r\n    </script>\r\n\r" +
    "\n</head>\r\n<body>\r\n    <!--side effect geek-->\r\n    <div class=\"sidebar\">\r\n      " +
    "  <div class=\"sidebar-head\"><img src=\"img/Logo128.png\" /><span> Dashboard</span>" +
    " </div>\r\n        <ul class=\"sidebar-menu scrollable pos-r ps ps--active-y\">\r\n   " +
    "         <li><a href=\"#null\"><i class=\"fa fa-home\"></i><span> News</span></a></l" +
    "i>\r\n        </ul>\r\n    </div>\r\n</div>\r\n\r\n<!-- Container -->\r\n<div class=\"contain" +
    "er tabular has-fixed-menubar\">\r\n        <div id=\"menubar\" class=\"fixed toolbar c" +
    "lear\">\r\n            <label for=\"show_menu\" class=\"show media\"><i class=\"fa fa-ba" +
    "rs\"></i> </label>\r\n            <input type=\"checkbox\" id=\"show_menu\" class=\"show" +
    " media\" role=\"button\">\r\n\r\n            <!-- App -->\r\n            <ul class=\"menu " +
    "hidden media fleft clear\">\r\n                <li><a href=\"/index.html?\"><i class=" +
    "\"fa fa-home\"></i></a></li>\r\n                <li><a href=\"#projects\">Bearbeiten</" +
    "a>\r\n                    <ul>\r\n                        <li><a href=\"#mountain\"></" +
    "a>Erstelle Kurse</li>\r\n                        <li><a href=\"#rail\">Erstelle Klas" +
    "sen</a></li>\r\n                        <li><a href=\"#\" onclick=\"TestSuS()\">Teste " +
    "SuS</a></li>\r\n                    </ul>\r\n                </li>\r\n            </ul" +
    ">\r\n\r\n\r\n            <!-- User -->\r\n            <ul class=\"menu info\">\r\n          " +
    "      <li><a href=\"https://aka.ms/mstfw\"><i class=\"fa fa-bell\" data-count=\"15\"><" +
    "/i></a></li>\r\n                <li><a href=\"https://outlook.office365.com/owa/?re" +
    "alm=gesamtschule-stolberg.de&exsvurl=1&ll-cc=1033&modurl=0\"><i class=\"fa fa-enve" +
    "lope\" data-count=\"4\"></i></a></li>\r\n                <li><a href=\"#logout\"><img i" +
    "d=\"userimage\" src=\"img/User.png\" /><span id=\"username\"> Username</span></a></li>" +
    "\r\n            </ul>\r\n        </div>\r\n    \r\n        <!-- App Content -->\r\n       " +
    " <div id=\"content\" class=\"section\">\r\n            <div class=\"tabular mcollapse\">" +
    "\r\n                <div class=\"box indent\">\r\n                    <div class=\"card" +
    "\">\r\n                        <h2>Edu365 Shell</h2>\r\n                        <h4>R" +
    "unning ... </h4>\r\n                        <div id=\"data\" style=\"overflow-y: scro" +
    "ll; height: 500px; width: 100%\">\r\n                            ... please press L" +
    "ogin\r\n                        </div>\r\n                        <label for=\"input\"" +
    ">Command Line:</label>\r\n                        <input id=\"input\" type=\"text\"  s" +
    "tyle=\"width: 100%\" autofocus>\r\n                    </div>\r\n                </div" +
    ">\r\n            </div>\r\n        </div>\r\n\r\n        \r\n        \r\n        <div class=" +
    "\"footer\">\r\n            <p>Copyright © 2018 <a href=\"mailto:benno.falkner@gmail.c" +
    "om\">Benjamin \'Benno\' Falkner</a>. All Rights reserved.</p>\r\n        </div>  \r\n  " +
    "  </div>\r\n</body>\r\n</html>");
        }
    }
}
