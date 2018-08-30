var Edu365 = (function(){
    var username = "";
    var id = "";
    var ready = false;

    // Init Edu365
    var init = function (Config) {
        Config.onSuccess = function(){
            username = ReCall.OAuth.Data['id_token'].name;
            id = ReCall.OAuth.Data['id_token'].oid;
            ready = true;
            if(Config.onLogin) { Config.onLogin(); }
        };
        ReCall.OAuth.Init(Config);
    }
    // get a resource
    var get = function(URL, onSuccess, onError){
        ReCall.OAuth.Request.Send(URL,{
            onSuccess: function(result){onSuccess(JSON.parse(result));},
            onError: function(Error,Code){onError("("+Code+") "+Error)}
        });
    }

    var getPhoto = function(onSuccess){
        ReCall.OAuth.Request.Send("https://graph.microsoft.com/beta/me/photo/$value",{
            onSuccess: function(result){
                console.log(typeof result)
                var url = window.URL || window.webkitURL;
                var blobUrl = url.createObjectURL(result);
                onSuccess(blobUrl);
            }
        });
    }

    // set a resource
    var set = function(Method, URL, Data, onSuccess, onError){
        ReCall.OAuth.Request.Send(URL,{
            Method: Method,
            onSuccess: function(result){console.log("Result: "+result);onSuccess((result != "" ? JSON.parse(result) : null));},
            onError: function(Error,Code){onError("("+Code+") " + Error)},
            Type: 'application/json',
            Body: Data
        });
    }
    // user 
    var user = (function(){
        return {
            Get: function(givenname, surname, onSuccess, onError){
                //get("https://graph.microsoft.com/v1.0/users?$filter=surname eq '"+surname+"' and givenname eq '"+ givenname+"'",onSuccess, onError);
                get("https://graph.microsoft.com/v1.0/users?$select=id,displayName,mail,UEAN&$filter=surname eq '"+surname+"' and givenname eq '"+ givenname+"'",onSuccess, onError);
            }
        }
    })();
    // team management
    var team = (function(){
        return {
            Get: function(name, onSuccess, onError){
                get("https://graph.microsoft.com/beta/groups/?$select=id,displayname,resourceProvisioningOptions&$filter=displayname eq '"+ name +"'", function(r){if(r.value.length == 0){onError(r);}else {onSuccess(r);}}, onError);
            },
            Create: function(name, onSuccess, onError){
                if (name) { 
                    get("https://graph.microsoft.com/beta/groups/?$select=id,displayname,resourceProvisioningOptions&$filter=displayname eq '"+ name +"'", 
                        function(result){ if(result && result.value.length > 0 ) {onError('Group exsists: '+result.value.displayName)} else {
                            console.log(result); 
                            set('POST','https://graph.microsoft.com/beta/groups','{"displayName": "'+name+'","groupTypes": ["Unified"],"mailEnabled": true,"mailNickname":"'+name.replace(/\s/g, "")+'","securityEnabled": false,"visibility":"Private"}', 
                                function(result) {
                                    console.log(result); 
                                    set('PUT','https://graph.microsoft.com/beta/groups/'+result.id+'/team','{"memberSettings": {"allowCreateUpdateChannels": false},"messagingSettings": {"allowUserEditMessages": true,"allowUserDeleteMessages": false},"funSettings": {"allowGiphy": true,"giphyContentRating": "strict"}}',
                                        onSuccess,
                                        onError
                                    );
                                }, onError);
                        }}, onError);
                } else {
                    onError("No Team Name");
                }
            },
            Delete: function(team,onSuccess, onError){
                set('Delete','https://graph.microsoft.com/beta/groups/'+team.value[0].id,null,onSuccess, onError);
            },
            GetOwner: function(team,onSuccess, onError){
                get('https://graph.microsoft.com/beta/groups/'+team.value[0].id+'/owners/?$select=id,displayName,mail',onSuccess, onError);
            },
            AddOwner: function(team,user,onSuccess, onError){
                set('POST','https://graph.microsoft.com/beta/groups/'+team.value[0].id+'/owners/$ref','{"@odata.id": "https://graph.microsoft.com/beta/users/'+user.value[0].id+'"}',onSuccess, onError);
            },
            RmOwner: function(team,user,onSuccess, onError){
                set('Delete','https://graph.microsoft.com/beta/groups/'+team.value[0].id+'/owners/'+user.value[0].id+'/$ref',null,onSuccess, onError);
            },
            GetMember: function(team,onSuccess, onError){
                get('https://graph.microsoft.com/beta/groups/'+team.value[0].id+'/members/?$select=id,displayName,mail',onSuccess, onError);
            },
            AddMember: function(team,user,onSuccess, onError){
                    set('POST','https://graph.microsoft.com/beta/groups/'+team.value[0].id+'/members/$ref','{"@odata.id": "https://graph.microsoft.com/beta/users/'+user.value[0].id+'"}',onSuccess, onError);
            },
            RmMember: function(team,user,onSuccess, onError){
                set('Delete','https://graph.microsoft.com/beta/groups/'+team.value[0].id+'/members/'+user.value[0].id+'/$ref',null,onSuccess, onError);
            }
        }
    })();

    var students = (function(){
        return {
            Check: function(Data,onSuccess,onError){
                var elem = Data.shift();
                if(elem == null || ReCall.IsEmpty(elem)) {onSuccess('done');return;}
                user.Get(elem.student_givenname,elem.student_surname,
                    function(r){
                        if(!r.value[0]){
                            onError(elem.student_givenname+" "+elem.student_surname+" ("+elem.class+") not found");
                        } else {
                            console.log(elem.student_givenname+" "+elem.student_surname+" ("+elem.class+") ok");
                        }
                        students.Check(Data,onSuccess,onError);}, 
                    onError);
            },
            Assign: function(Data,naming,onSuccess,onError){
                var elem = Data.shift();
                if(elem == null || ReCall.IsEmpty(elem)) {onSuccess('done');return;}
                var name = naming(elem);
                if(name == undefined ) {students.Assign(Data,naming,onSuccess,onError); return;}
                console.log("processing:" + elem.student_givenname+" "+elem.student_surname +" "+name);
                team.Get(name,
                    function(_team){
                        if( _team.value.length == 0){onError(name+ " resolving course for ("+ elem.student_givenname+" "+elem.student_surname +") failed.");students.Assign(Data,naming,onSuccess,onError);return}
                        console.log("team:" + _team.value[0].displayName);
                        user.Get(elem.student_givenname,elem.student_surname,
                            function(_user){
                                if( _user.value.length == 0){onError(name+ " setting member ("+ elem.student_givenname+" "+elem.student_surname +") failed.");students.Assign(Data,naming,onSuccess,onError);return}
                                console.log("user: "+_user.value[0].displayName);
                                team.AddMember(_team,_user,
                                    function(r){onSuccess(name+ " user: "+elem.student_givenname+" "+elem.student_surname+" added.");students.Assign(Data,naming,onSuccess,onError);},
                                    function(r){console.log(r);onError(name+ " user: "+elem.student_givenname+" "+elem.student_surname+" adding failed.");students.Assign(Data,naming,onSuccess,onError);});},
                            function (r){onError(name+ " user: "+elem.student_givenname+" "+elem.student_surname+" not found.");students.Assign(Data,naming,onSuccess,onError)});},
                    function (r){onError(name+ " not found.");students.Assign(Data,naming,onSuccess,onError)});
            }
        }
    })();
    
    var faculty = (function(){
        return {
            Check: function(Data,onSuccess,onError){
                var elem = Data.shift();
                if(elem == null || ReCall.IsEmpty(elem)) {onSuccess('done');return;}
                Edu365.User.Get(elem.teacher_givenname,elem.teacher_surname,
                    function(r){
                        if(!r.value[0]){
                            onError(elem.teacher_givenname+" "+elem.teacher_surname+" not found");
                        } else {
                            console.log(elem.teacher_givenname+" "+elem.teacher_surname+ " ok");
                        }
                        faculty.Check(Data,onSuccess,onError);}, 
                    onError);
            },
            CreateTeam: function(Data,naming,onSuccess,onError){
                var elem = Data.shift();
                if(elem == null || ReCall.IsEmpty(elem)) {onSuccess('done');return;}
                var name = naming(elem);
                if(name == undefined ) {faculty.CreateTeam(Data,naming,onSuccess,onError); return;}
                team.Create(name,
                    function (_team){ 
                        user.Get(elem.teacher_givenname,elem.teacher_surname, 
                            function (_user){
                                if( _user.value.length == 0){onError(name+ " setting owner ("+ elem.teacher +") failed.");faculty.CreateTeam(Data,naming,onSuccess,onError);return }
                                team.AddOwner({value: [_team]},_user,
                                    function(r){if(_user.value[0].id != id) {
                                            console.log("removing me: "+id)
                                            team.RmOwner({value: [_team]},{value: [{id: id}]},
                                                function(r){onSuccess(_team);faculty.CreateTeam(Data,naming,onSuccess,onError)},
                                                function(r){onError(name+ " removing me failed.");faculty.CreateTeam(Data,naming,onSuccess,onError)});
                                        }else{
                                            onSuccess(_team);faculty.CreateTeam(Data,naming,onSuccess,onError)}},
                                    function (r){onError(name+ " setting owner ("+ elem.teacher +") failed.");faculty.CreateTeam(Data,naming,onSuccess,onError)})},
                            function (r){onError(name+ " getting owner ("+ elem.teacher +") failed.");faculty.CreateTeam(Data,naming,onSuccess,onError)})},
                    function (r){onError(name+ " creation failed.");faculty.CreateTeam(Data,naming,onSuccess,onError)});
            },

            CheckTeams: function(Data,naming,onSuccess,onError){
                var elem = Data.shift();
                if(elem == null || ReCall.IsEmpty(elem)) {onSuccess('done');return;}
                var name = naming(elem);
                if(name == undefined ) {faculty.CheckTeams(Data,naming,onSuccess,onError); return;}
                team.Get(name,
                    function(r){
                        if(!r.value[0]){
                            onError(name +" not found");
                        } else {
                            console.log(name+ " ok");
                        }
                        faculty.CheckTeams(Data,naming,onSuccess,onError);}, 
                    onError);
            }


        }
    })();


    return {
        Init: init,
        Login: ReCall.OAuth.Login,
        Username: function(){return username;},
        onError: function(result){console.log(result);},
        IsReady: function(){return ready;},
        Photo: getPhoto,

        Get: get,
        Set: set,

        User: user,
        Team: team,
        Students: students,
        Faculty: faculty
    }
})();