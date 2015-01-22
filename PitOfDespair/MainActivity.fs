namespace PitOfDespair

open System

open Android.App
open Android.Content
open Android.OS
open Android.Runtime
open Android.Views
open Android.Widget

[<Activity (Label = "PitOfDespair", MainLauncher = true)>]
type MainActivity () =
    inherit Activity ()

    let mutable strength:int = 1
    let mutable dex:int = 1
    let mutable intelligence:int = 1
    let mutable characterClass:int = 1
    let changeClass c () = characterClass <- c
    let ignoreArgs f : EventArgs->unit = fun args -> f()
    let onClick f (btn:Button) = btn.Click.Add (fun args -> f())
    let withClick f (btn:Button) = btn.Click.Add f
    member private  this.gotoMain () = 
        this.SetContentView (Resource_Layout.Main)
        let btnChooser = this.FindViewById<Button>(Resource_Id.btnChooser)
        btnChooser |> onClick this.gotoChooser
        btnChooser.Text <- characterClass.ToString()
        this.FindViewById<Button>(Resource_Id.btnAdventure)
        |> onClick (fun () -> 
            let activity = new Intent(this,typeof<Dungeon>)
            this.StartActivityForResult(activity,0)

        )
        
    override x.OnActivityResult (requestCode,resultCode,data) =
        if resultCode = Result.Ok then
            characterClass <- data.GetIntExtra(ClassPicker.Key,characterClass)
            Diagnostics.Debug.WriteLine("yay activity result was ok")
            let btnChooser = x.FindViewById<Button>(Resource_Id.btnChooser)
            btnChooser.Text <- "Class:" + characterClass.ToString()

    member private this.gotoChooser () = 
        let activity = new Intent(this,typeof<ClassPicker>)
        this.StartActivityForResult(activity,characterClass)

    override this.OnCreate (bundle) =
        base.OnCreate (bundle)
        // Set our view from the "main" layout resource
        this.gotoMain()

    
