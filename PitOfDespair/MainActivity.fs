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
    let onClick f (btn:Button) = btn.Click.Add f
    member private  this.gotoMain () = 
        this.SetContentView (Resource_Layout.Main)
        let button = this.FindViewById<Button>(Resource_Id.btnChooser)
        button.Click.Add (fun args -> this.gotoChooser())

    member private this.gotoChooser () = 
        //button.Text <- sprintf "%d clicks!" count
        //count <- count + 1
        this.SetContentView(Resource_Layout.Classes)
        this.FindViewById<Button>(Resource_Id.btnWarrior)
        |> onClick (ignoreArgs <| changeClass 2 >> this.gotoMain)
        this.FindViewById<Button>(Resource_Id.btnAdventurer)
        |> onClick (ignoreArgs <| changeClass 1 >> this.gotoMain)
    override this.OnCreate (bundle) =
        base.OnCreate (bundle)
        // Set our view from the "main" layout resource
        this.gotoMain()

    
