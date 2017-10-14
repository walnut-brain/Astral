namespace Astral.Schema
open System
open System.Reflection

type ISchemaExtender =
    abstract Name : string
    abstract ExtendServiceByType : serviceType: Type *  option: Map<string, DataItem>  -> Result<Map<string, DataItem>, string> 
    abstract ExtendEventByProperty : eventProperty: PropertyInfo * option : Map<string, DataItem> -> Result<Map<string, DataItem>, string>
    abstract ExtendCallByProperty : eventProperty: PropertyInfo * option : Map<string, DataItem> -> Result<Map<string, DataItem>, string>
    