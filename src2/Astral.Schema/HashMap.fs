namespace rec Astral.Schema
open System.Collections
open System.Collections.Generic
open System.Collections.Immutable
open System.Collections.Generic
open System.Runtime.InteropServices

            
module HashMap =
    let empty<'key, 'value> = ImmutableDictionary<'key, 'value>.Empty
    let createS<'key, 'value> values =
        ImmutableDictionary.CreateRange<'key, 'value>(values |> Seq.map (fun struct (k, v) -> KeyValuePair(k, v)))
    let create<'key, 'value> values =
        ImmutableDictionary.CreateRange<'key, 'value>(values |> Seq.map (fun (k, v) -> KeyValuePair(k, v)))
    let createKV<'key, 'value> values =
        ImmutableDictionary.CreateRange<'key, 'value>(values)
    let add (k : 'key)  (v: 'value) (map : ImmutableDictionary<'key, 'value>) = map.Add(k, v) 
    let addRange pairs (map : ImmutableDictionary<'key, 'value>) =
        map.AddRange(pairs |> Seq.map (fun (k, v) -> KeyValuePair(k, v))) 
    let addRangeS pairs (map : ImmutableDictionary<'key, 'value>) =
        map.AddRange(pairs |> Seq.map (fun struct (k, v) -> KeyValuePair(k, v))) 
    let addRangeKV pairs (map : ImmutableDictionary<'key, 'value>) =
        map.AddRange(pairs)             
    let set (k : 'key)  (v: 'value) (map : ImmutableDictionary<'key, 'value>) =
        map.SetItem(k, v) 
    let setValues values (map : ImmutableDictionary<'key, 'value>) =
        map.SetItems(values |> Seq.map (fun (k, v) -> KeyValuePair(k, v))) 
    let setValuesS values (map : ImmutableDictionary<'key, 'value>) =
        map.SetItems(values |> Seq.map (fun struct (k, v) -> KeyValuePair(k, v))) 
    let setValuesKV values (map : ImmutableDictionary<'key, 'value>) =
        map.SetItems(values) 
             
type HashMap<'k, 'v> = ImmutableDictionary<'k, 'v>    


    
    
     