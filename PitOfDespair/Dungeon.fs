
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
[<Measure>]
type X
[<Measure>]
type Y

type CellState = 
    |Closed
    |Entrance
    |HasPlayer
    |Explored
    |Exit

type Encounter =
    | Monster
    | Treasure
type GridCell = Button
type GridCoord<[<Measure>]'a> = int<'a>
type GridX = GridCoord<X>
type GridY = GridCoord<Y>

[<Activity (Label = "Dungeon")>]
type Dungeon() =
  inherit Activity()
  let mutable posX,posY = 1<X>,1<Y>
  let measuredRange min (max) :int<'a> list = 
      [ min / 1<_> .. max / 1<_>]
      |> List.map( fun f -> LanguagePrimitives.Int32WithMeasure f * LanguagePrimitives.GenericOne)
  static member MaxX = 5<X>
  static member MaxY = 5<Y>
  static member Range<[<Measure>] 'a> (min:int<'a>,max:int<'a>) = 
      [ int min .. int max]
  static member MeasuredRange<[<Measure>]'a> (min:int<'a>) max :int<'a> list = 
      Dungeon.Range (min,max)
      |> List.map (fun f -> LanguagePrimitives.Int32WithMeasure<'a> f )
  member this.CellText x y state :string = 
    let stateRep = 
        if (x,y) = (posX,posY) then "P" else
        match state with
           |Some HasPlayer -> "P"
           |Some Entrance -> "_"
           |Some Exit -> "X"
           |Some Explored -> String.Empty
           | _ -> "C"

    stateRep + ":"+ x.ToString() + "," + y.ToString()

  member private this.RegenRow (y, row:TableRow):unit =
      let rawY = int y
      let view = row.GetChildAt rawY :?> GridCell
      for x in Dungeon.MeasuredRange 0<X> (Dungeon.MaxX - 1<X>) do
        let rawX = int x
        let text = this.CellText x y (if posX = x && posY=y then Some CellState.HasPlayer else Some CellState.Closed)
        view.Text <- text
 

  member private this.InitializeTable (tbl:TableLayout) =
    let doRegen = tbl.ChildCount <> 0
    let maxY = if doRegen then tbl.ChildCount * 1<Y> else Dungeon.MaxY
    for y in Dungeon.MeasuredRange<Y> LanguagePrimitives.GenericZero (maxY - 1<Y>) do

        let row = if doRegen then 
                    tbl.GetChildAt( int y ) :?> TableRow 
                    else 
                        let r = new TableRow(this)
                        tbl.AddView(r)
                        r
        if row = null then failwith "row did not return"
        let maxX = if row.ChildCount = 0 then Dungeon.MaxX else row.ChildCount * 1<X>
        if row.ChildCount > 0 then
            for x in Dungeon.MeasuredRange<X> LanguagePrimitives.GenericZero (maxX - 1<X>) do
                //reinitialize table for new floor
                let text = this.CellText x y None
                let view:GridCell = this.getView x y
                view.Text <- text
        else
            this.InitializeRow(y,row)
  member private this.InitializeRow (y,row:TableRow) = 
    for x in Dungeon.MeasuredRange<X> LanguagePrimitives.GenericZero (Dungeon.MaxX - 1<X>) do
        let view = new GridCell(context = this,Text = this.CellText x y (if posX = x && posY=y then Some CellState.HasPlayer else None))
        row.AddView view

  member private this.getStartLocation () = 
      0,0

  member private this.getRow y = 
      let table = this.FindViewById<TableLayout>(Resource_Id.tableLayout1)
      table.GetChildAt y :?> TableRow

  member private this.getView x y = 
      let rawY = int y
      let rawX = int x
      let row = this.getRow rawY
      row.GetChildAt rawX :?> GridCell
  
  override this.OnCreate(bundle) =
    base.OnCreate (bundle)

    // Create your application here
    this.SetContentView(Resource_Layout.Dungeon)
    let table = this.FindViewById<TableLayout>(Resource_Id.tableLayout1)
    this.InitializeTable(table)


