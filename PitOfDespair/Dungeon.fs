
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
type Encounter =
    | Monster
    | Treasure
[<Activity (Label = "Dungeon")>]
type Dungeon() =
  inherit Activity()

  override this.OnCreate(bundle) =
    base.OnCreate (bundle)
    // Create your application here
    this.SetContentView(Resource_Layout.Dungeon)
    let table = this.FindViewById<TableLayout>(Resource_Id.tableLayout1)
    for y in [0..table.ChildCount-1] do
        let row = table.GetChildAt(y) :?> TableRow
        if row.ChildCount > 0 then
            for x in [0 .. row.ChildCount-1] do
                ()
        else
            for x in [0..4] do
                let view = new Button(context =this,Text= x.ToString()+","+y.ToString())
                row.AddView( view)


