// main.cs
// Copyright 2018 Benjamin 'Benno' Falkner. All rights reserved.
// Use of this source code is governed by a MIT-style
// license that can be found in the LICENSE file.

using System;
using System.Data.OleDb;
using System.Collections.Generic;


public class SchuleNRW365:Pink.Handler {
    static string SEP = ";";
    Pink.DBServer DB;
    OleDbConnection db;
    Pink.Router routes;
    Pink.Templates tmpl;
    Pink.StaticFileHandler staticHandler;
    Pink.Server srv;

    public SchuleNRW365(){
        routes = new Pink.Router();
        tmpl = new Pink.Templates();
    }

    public void Init(){
        Console.WriteLine("Read Config ...");
        Config cfg = Config.Read(@".cfg");

        // helper to check config
        foreach (KeyValuePair<string, string> kv in cfg)
            Console.WriteLine(">>> " + kv.Key + " = " + kv.Value);
        
        Console.WriteLine("Open DB ...");
        // DB connection
        string provider = Pink.DB.GetProvider("ACE");
        if(provider==null) { Console.WriteLine("Error: No ACE Provider"); return;}    
        db = Pink.DB.Connect(provider,cfg["Database"]);
        DB = new Pink.DBServer(db);
        DB.Start();

        Console.WriteLine("Start Webserver ... ");

        staticHandler = new Pink.StaticFileHandler(cfg["StaticFiles"]);
        //routes.Add(cfg["Server"]+"index.html", staticHandler);
        routes.Add(cfg["Server"]+"img/", staticHandler);
        routes.Add(cfg["Server"]+"css/", staticHandler);
        routes.Add(cfg["Server"]+"js/", staticHandler);
        routes.Add(cfg["Server"]+"favicon.ico", staticHandler);
        routes.Add(cfg["Server"]+"schild/", this);
        routes.Add(cfg["Server"]+"index.html", tmpl.fromFile("INDEX",cfg["IndexFile"]));

        srv = new Pink.Server(cfg["Server"],routes);
        srv.Start();
    }

    public void Run(){
        Console.WriteLine("Run");
    }
    public void Quit(){
        srv.Stop();
        DB.Stop();
        Console.WriteLine("... Bye.");
    }
    public override void handle(Pink.Request req){
        if(req.URL == "/schild/students.json"){
            req_students(req, Pink.MIME.JSON, -1);
        } else if(req.URL == "/schild/faculty.json"){
            req_faculty(req, Pink.MIME.JSON, -1);
        } else if(req.URL == "/schild/classes.json"){
            req_classes(req, Pink.MIME.JSON, -1);
        } else if(req.URL == "/schild/courses.json"){
            req_courses(req, Pink.MIME.JSON,  -1);
        } else if(req.URL == "/schild/allocation.json"){
            req_allocation(req, Pink.MIME.JSON, -1);
        } else if(req.URL == "/schild/s2/courses.json"){
            req_courses_s2(req, Pink.MIME.JSON,  -1);
        } else if(req.URL == "/schild/s2/allocation.json"){
            req_allocation_s2(req, Pink.MIME.JSON, -1);
        } else if(req.URL == "/schild/students.csv"){
            req_students(req, Pink.MIME.CSV, -1);
        } else if(req.URL == "/schild/faculty.csv"){
            req_faculty(req, Pink.MIME.CSV, -1);
        } else if(req.URL == "/schild/classes.csv"){
            req_classes(req, Pink.MIME.CSV, -1);
        } else if(req.URL == "/schild/courses.csv"){
            req_courses(req, Pink.MIME.CSV,  -1);
        } else if(req.URL == "/schild/allocation.csv"){
            req_allocation(req, Pink.MIME.CSV, -1);
        } else if(req.URL == "/schild/s2/courses.csv"){
            req_courses_s2(req, Pink.MIME.CSV,  -1);
        } else if(req.URL == "/schild/s2/allocation.csv"){
            req_allocation_s2(req, Pink.MIME.CSV, -1);
        } else {
            req.StatusCode = 200;
            req.ContentType = Pink.MIME.JSON;
            req.WriteString(string.Format("{{\"success\":true,\"url\":\"{0}\",\"info\":\"default json test response\"}}",req.URL));
        }
    }
    private void req_students(Pink.Request req, string format, int ean){
        string stm = "SELECT Schueler.ID, Schueler.Vorname, Schueler.Name, SchuelerLernabschnittsdaten.Klasse, SchuelerLernabschnittsdaten.ASDJahrgang " +
                        "FROM EigeneSchule, Schueler INNER JOIN SchuelerLernabschnittsdaten ON Schueler.ID = SchuelerLernabschnittsdaten.Schueler_ID " +
                        "WHERE (((SchuelerLernabschnittsdaten.Jahr)=[EigeneSchule].[Schuljahr]) AND ((SchuelerLernabschnittsdaten.Abschnitt)=[EigeneSchule].[SchuljahrAbschnitt]));";
        OleDbDataReader r = DB.Query(stm,100);
        string[] header = new string[] {"id","student_givenname","student_surname","class","grade"};


        req.StatusCode = 200;
        if(format == Pink.MIME.CSV){
            req.ContentType = Pink.MIME.CSV;
            Pink.CSV.Write(r,req.Output,SEP,header,ean);
        } else {
            req.ContentType = Pink.MIME.JSON;
            Pink.JSON.Write(r,req.Output,header,ean);
        }
        r.Close();
        return ;
    }
    private void req_faculty(Pink.Request req, string format, int ean){
        string stm = "SELECT K_Lehrer.ID, K_Lehrer.Kuerzel, K_Lehrer.Vorname, K_Lehrer.Nachname " +
                        "FROM K_Lehrer " +
                        "WHERE (((K_Lehrer.Kuerzel) Is Not Null) AND ((K_Lehrer.Vorname) Is Not Null) AND ((K_Lehrer.Nachname) Is Not Null) AND ((K_Lehrer.Sichtbar)='+')); ";
        OleDbDataReader r = DB.Query(stm,100);
        string[] header = new string[] {"id","teacher","teacher_givenname","teacher_surname"};

        req.StatusCode = 200;
        if(format == Pink.MIME.CSV){
            req.ContentType = Pink.MIME.CSV;
            Pink.CSV.Write(r,req.Output,SEP,header,ean);
        } else {
            req.ContentType = Pink.MIME.JSON;
            Pink.JSON.Write(r,req.Output,header,ean);
        }
        r.Close();
        return ;
    }
    private void req_classes(Pink.Request req, string format, int ean){
        string stm = "SELECT DISTINCT SchuelerLernabschnittsdaten.Klasse, K_Lehrer.Vorname, K_Lehrer.Nachname, K_Lehrer.Kuerzel " +
                        "FROM EigeneSchule, (Schueler INNER JOIN K_Lehrer ON Schueler.Lehrer = K_Lehrer.Kuerzel) INNER JOIN SchuelerLernabschnittsdaten ON Schueler.ID = SchuelerLernabschnittsdaten.Schueler_ID "+
                        "WHERE (((SchuelerLernabschnittsdaten.KlassenLehrer) Is Not Null) AND ((SchuelerLernabschnittsdaten.Jahr)=[EigeneSchule].[Schuljahr]) AND ((SchuelerLernabschnittsdaten.Abschnitt)=[EigeneSchule].[SchuljahrAbschnitt]));";
        OleDbDataReader r = DB.Query(stm,100);
        string[] header = new string[] {"class","teacher_givenname","teacher_surname","teacher"};

        req.StatusCode = 200;
        if(format == Pink.MIME.CSV){
            req.ContentType = Pink.MIME.CSV;
            Pink.CSV.Write(r,req.Output,SEP,header,-1);
        } else {
            req.ContentType = Pink.MIME.JSON;
            Pink.JSON.Write(r,req.Output,header,-1);
        }
        r.Close();
        return ;
    }
    private void req_courses(Pink.Request req, string format, int ean){

        string stm = "SELECT DISTINCT EigeneSchule_Faecher.FachKrz, EigeneSchule_Faecher.Bezeichnung, SchuelerLeistungsdaten.Kursart, SchuelerLernabschnittsdaten.Klasse, SchuelerLernabschnittsdaten.ASDJahrgang, K_Lehrer.Vorname, K_Lehrer.Nachname, SchuelerLeistungsdaten.FachLehrer " +
                        "FROM EigeneSchule, (Schueler INNER JOIN (SchuelerLernabschnittsdaten INNER JOIN (EigeneSchule_Faecher INNER JOIN SchuelerLeistungsdaten ON EigeneSchule_Faecher.ID = SchuelerLeistungsdaten.Fach_ID) ON SchuelerLernabschnittsdaten.ID = SchuelerLeistungsdaten.Abschnitt_ID) ON Schueler.ID = SchuelerLernabschnittsdaten.Schueler_ID) INNER JOIN K_Lehrer ON SchuelerLeistungsdaten.FachLehrer = K_Lehrer.Kuerzel " +
                        "WHERE (((SchuelerLernabschnittsdaten.Jahr)=[EigeneSchule].[Schuljahr]) AND ((SchuelerLernabschnittsdaten.Abschnitt)=[EigeneSchule].[SchuljahrAbschnitt]));";
        OleDbDataReader r = DB.Query(stm,100);
        string[] header = new string[] {"subject","subject_description","subject_type","class","grade","teacher_givenname","teacher_surname","teacher"};



        req.StatusCode = 200;
        if(format == Pink.MIME.CSV){
            req.ContentType = Pink.MIME.CSV;
            Pink.CSV.Write(r,req.Output,SEP,header,-1);
        } else {
            req.ContentType = Pink.MIME.JSON;
            Pink.JSON.Write(r,req.Output,header,-1);
        }
        r.Close();
        return ;
    }
    private void req_allocation (Pink.Request req, string format, int ean){

        string stm = "SELECT Schueler.Vorname, Schueler.Name, EigeneSchule_Faecher.FachKrz,  SchuelerLeistungsdaten.Kursart, SchuelerLernabschnittsdaten.Klasse, SchuelerLernabschnittsdaten.ASDJahrgang, SchuelerLeistungsdaten.FachLehrer " +
                        "FROM EigeneSchule, Schueler INNER JOIN (SchuelerLernabschnittsdaten INNER JOIN (EigeneSchule_Faecher INNER JOIN SchuelerLeistungsdaten ON EigeneSchule_Faecher.ID = SchuelerLeistungsdaten.Fach_ID) ON SchuelerLernabschnittsdaten.ID = SchuelerLeistungsdaten.Abschnitt_ID) ON Schueler.ID = SchuelerLernabschnittsdaten.Schueler_ID " +
                        "WHERE (((SchuelerLernabschnittsdaten.Jahr)=[EigeneSchule].[Schuljahr]) AND ((SchuelerLernabschnittsdaten.Abschnitt)=[EigeneSchule].[SchuljahrAbschnitt])) ORDER BY (Schueler.Name) ;";
        OleDbDataReader r = DB.Query(stm,100);
        string[] header = new string[] {"student_givenname","student_surname","subject","subject_type","class","grade","teacher"};


        if(req.Query["start"]!= null){
            int x = Int32.Parse(req.Query["start"]);
            for(int i=0;i<x;i++)
                r.Read();
        }
        req.StatusCode = 200;
        if(format == Pink.MIME.CSV){
            req.ContentType = Pink.MIME.CSV;
            Pink.CSV.Write(r,req.Output,SEP,header,-1);
        } else {
            req.ContentType = Pink.MIME.JSON;
            Pink.JSON.Write(r,req.Output,header,-1);
        }
        r.Close();
        return ;
    }

    private void req_courses_s2 (Pink.Request req, string format, int ean){

        string stm = "SELECT Kurse.KurzBez, Kurse.LehrerKrz, K_Lehrer.Vorname, K_Lehrer.Nachname, Kurse.ASDJahrgang " +
                        "FROM EigeneSchule, Kurse INNER JOIN K_Lehrer ON Kurse.LehrerKrz = K_Lehrer.Kuerzel " +
                        "WHERE (((Kurse.ASDJahrgang)='EF' Or (Kurse.ASDJahrgang)='Q1' Or (Kurse.ASDJahrgang)='Q2') AND ((Kurse.Jahr)=[EigeneSchule].[Schuljahr]) AND ((Kurse.Abschnitt)=[EigeneSchule].[SchuljahrAbschnitt]));";
        OleDbDataReader r = DB.Query(stm,100);
        string[] header = new string[] {"subject","teacher","teacher_givenname","teacher_surname","grade"};

        req.StatusCode = 200;
        if(format == Pink.MIME.CSV){
            req.ContentType = Pink.MIME.CSV;
            Pink.CSV.Write(r,req.Output,SEP,header,-1);
        } else {
            req.ContentType = Pink.MIME.JSON;
            Pink.JSON.Write(r,req.Output,header,-1);
        }
        r.Close();
        return ;
    }

    private void req_allocation_s2 (Pink.Request req, string format, int ean){
        string stm = "SELECT Schueler.ID, Schueler.Vorname, Schueler.Name, SchuelerLernabschnittsdaten.Klasse, Kurse.KurzBez, Kurse.LehrerKrz " +
                        "FROM EigeneSchule, (Schueler INNER JOIN (SchuelerLernabschnittsdaten INNER JOIN SchuelerLeistungsdaten ON SchuelerLernabschnittsdaten.ID = SchuelerLeistungsdaten.Abschnitt_ID) ON Schueler.ID = SchuelerLernabschnittsdaten.Schueler_ID) INNER JOIN Kurse ON SchuelerLeistungsdaten.Kurs_ID = Kurse.ID " +
                        "WHERE (((SchuelerLernabschnittsdaten.Jahr)=[EigeneSchule].[Schuljahr]) AND ((SchuelerLernabschnittsdaten.Abschnitt)=[EigeneSchule].[SchuljahrAbschnitt]) AND ((SchuelerLernabschnittsdaten.Klasse)='EF' Or (SchuelerLernabschnittsdaten.Klasse)='Q1' Or (SchuelerLernabschnittsdaten.Klasse)='Q2'));";
        OleDbDataReader r = DB.Query(stm,100);
        string[] header = new string[] {"id","student_givenname","student_surname","grade","subject","teacher"};

        req.StatusCode = 200;
        if(format == Pink.MIME.CSV){
            req.ContentType = Pink.MIME.CSV;
            Pink.CSV.Write(r,req.Output,SEP,header,-1);
        } else {
            req.ContentType = Pink.MIME.JSON;
            Pink.JSON.Write(r,req.Output,header,-1);
        }
        r.Close();
        return ;
    }



    public static void Main (string[] args){
        try {
            Console.WriteLine("SchuleNRW365 Server. Press a key to quit.");
            SchuleNRW365 s = new SchuleNRW365();
            s.Init();
            s.Run();
            Console.ReadKey();
            s.Quit();
        } catch (Exception e){
            Console.WriteLine(e.ToString());
        }
    }
}



/** Read and store simple config in a Dictionary */
public class Config:Dictionary<string, string>{
    public static Config Read(string filen){
        Config c = new Config();
        string line;
        System.IO.StreamReader file = new System.IO.StreamReader(filen);  
        while((line = file.ReadLine()) != null)  {
            line = line.Trim();
            string[] comment = line.Split('#');
            string[] kv = comment[0].Split('=');

            if (kv.Length > 1 && kv[0] != "" && kv[1] !="") {
                c.Add(kv[0].Trim(),kv[1].Trim());
            }
        }  
        file.Close(); 
        return c;
    }
}