
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
    | ForceDown
    | Down

type GridCell = Button
type GridCoord<[<Measure>]'a> = int<'a>
type GridX = GridCoord<X>
type GridY = GridCoord<Y>

[<Activity (Label = "Dungeon")>]
type Dungeon() =
  inherit Activity()
  let mutable posX,posY,floor = 1<X>,1<Y>,1
  let floorState = new System.Collections.Generic.Dictionary<int<X>*int<Y>,CellState>() 
  let rnd = System.Random()
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

  member this.canGo (x:int<_>) y = 
      // from east
    let canFrom x y = 
          if x < 0<X> || y < 0<Y> then false
          else
            let view = this.getView x y 
            match view with
            | Some (gc:GridCell) -> gc.Text.StartsWith("C")=false
            | _ -> false
    canFrom (x-1<X>) y
    || canFrom (x+1<X>) y
    || canFrom x (y-1<Y>)
    || canFrom x (y+1<Y>)
        
  member this.CellState x y state =
    if (x,y) = (posX,posY) then HasPlayer else
        match state with
        |Some x -> x
        | _ -> Closed
        
  member this.CellText x y state :CellState*string = 
    let cellState = this.CellState x y state

    cellState,match cellState with
               |HasPlayer -> "P:"
               |Entrance -> "_:"
               |Exit -> "X:"
               |Explored -> String.Empty
               | _ -> "C:"
            + x.ToString() + "," + y.ToString()

  member private this.RegenRow (y, row:TableRow):unit =
      let rawY = int y
      let view = row.GetChildAt rawY :?> GridCell
      for x in Dungeon.MeasuredRange 0<X> (Dungeon.MaxX - 1<X>) do
        let rawX = int x
        let state,text = this.CellText x y (if posX = x && posY=y then Some CellState.HasPlayer else Some CellState.Closed)
        floorState.[(x,y)] <- state
        view.Text <- text
  member private this.combat floor = 
      ()
  member private this.treasure floor =
      ()
  member private this.explore cellState = 
      let isFight = rnd.Next(100)>50

      if isFight then
        this.combat floor

      let values:CellState list = floorState.Values |> Seq.toList

      let remaining = values |> Seq.filter( fun (v:CellState) -> v = CellState.Closed) |> Seq.length


      let exitFactor = rnd.Next(floorState.Keys.Count)> remaining
      let isForceExit = true // exitFactor && rnd.Next(1) > 0
      if exitFactor && isForceExit then
        let table = this.FindViewById<TableLayout>(Resource_Id.tableLayout1)
        let locX,locY = this.getStartLocation()
        posX <- locX
        posY <- locY
        this.InitializeTable table
        floor <- floor + 1
        CellState.Exit
      else
        this.treasure floor
        CellState.HasPlayer
  //let 
  member private this.onClick (target:GridCell) x y (oldState:CellState) =
     let oldStateText = sprintf "%A" oldState
     if this.canGo x y then
        let oldX,oldY = posX,posY

        printfn "going to %A from %A" (x,y) (posX,posY)
        posX <- x
        posY <- y
        target.SetOnClickListener(null)
        let _,prevNewText = this.CellText oldX oldY <| Some CellState.Explored
        let oldTarget:GridCell = (this.getView oldX oldY).Value
        oldTarget.Text <- prevNewText
        let state, text = this.CellText x y None
        target.Text <- text
        floorState.[(x,y)] <- this.explore state
     else
        System.Diagnostics.Debug.WriteLine <| sprintf "could not go to %A from %A" (x,y) (posX,posY)
     // match oldState with
     // | Closed when 
     // |
     ()
  
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

                let state,text = this.CellText x y None
                let view:GridCell = (this.getView x y).Value
                view.Text <- text
                floorState.[(x,y)] <- state
                view.Click.Add (fun args -> this.onClick view x y state)
        else
            this.InitializeRow(y,row)

  member private this.InitializeRow (y,row:TableRow) = 
    for x in Dungeon.MeasuredRange<X> LanguagePrimitives.GenericZero (Dungeon.MaxX - 1<X>) do
        let state,text = this.CellText x y (if posX = x && posY=y then Some CellState.HasPlayer else None)
        let view = new GridCell(context = this,Text = text)
        floorState.[(x,y)] <- state
        view.Click.Add (fun args -> this.onClick view x y state)
        row.AddView view

  member private this.getStartLocation () = 
      0<X>,0<Y>

  member private this.getRow y = 
      let table = this.FindViewById<TableLayout>(Resource_Id.tableLayout1)
      let row = table.GetChildAt y :?> TableRow
      if row <> null then Some row else None


  member private this.getView (x:int<_>) y = 
      let rawY = int y
      let rawX = int x
      let row = this.getRow rawY
      if row.IsSome then
        let gridCell = row.Value.GetChildAt rawX :?> GridCell
        if gridCell <> null then Some gridCell else None
      else None
  
  override this.OnCreate(bundle) =
    base.OnCreate (bundle)

    // Create your application here
    this.SetContentView(Resource_Layout.Dungeon)
    let table = this.FindViewById<TableLayout>(Resource_Id.tableLayout1)
    this.InitializeTable(table)


