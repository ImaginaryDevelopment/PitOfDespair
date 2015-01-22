
namespace PitOfDespair

open System
open System.Collections.Generic
open System.Linq
open System.Text

open Android.App
open Android.Content
open Android.OS
open Android.Runtime
open Android.Views
open Android.Widget

[<Activity (Label = "ClassPicker")>]
type ClassPicker() =
  inherit Activity()
  let mutable characterClass:int = 1

  let onClick f (btn:Button) = btn.Click.Add (fun args -> f())
  (* end let block *)
  static member Key = "characterClass"
  override x.OnSaveInstanceState(bundle) = 
    base.OnSaveInstanceState bundle
    bundle.PutInt(ClassPicker.Key, characterClass)

  override x.OnCreate(bundle) =
    base.OnCreate (bundle)
    // Create your application here
    //button.Text <- sprintf "%d clicks!" count
    //count <- count + 1
    if bundle <> null then
        characterClass <- bundle.GetInt(ClassPicker.Key)
    x.SetContentView(Resource_Layout.Classes)
    x.FindViewById<Button>(Resource_Id.btnWarrior)
    |> onClick (fun () -> 
        let intent = new Intent(x,typeof<ClassPicker>)
        x.SetResult(Result.Ok, intent.PutExtra(ClassPicker.Key,2))
        x.Finish() 
        )

    x.FindViewById<Button>(Resource_Id.btnAdventurer)
    |> onClick (fun () -> 
        let intent = new Intent(x,typeof<ClassPicker>)
        x.SetResult(Result.Ok, intent.PutExtra(ClassPicker.Key,1))
        x.Finish() 
        )
