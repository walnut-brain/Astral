### Lawium
The simple library to build hierarchical tree of inferences.

#### Sample of usage

```csharp
var builder = new LawBookBuilder();
builder.AddLaw(Law.Axiom(1));
var chieldBuilder = builder.GetSubBookBuilder("foo");
chieldBuilder.AddLaw(Law.Create((int i) => i.ToString()));
chieldBuilder = builder.GetSubBookBuilder("bar");
chieldBuilder.AddLaw(Law.Create((int i) => (i+1).ToString());
var book = builder.Build();

Assert.Equal(bool.TryGet<string>(), None);
Assert.Equal(book.GetOrAddSubBook("foo").TryGet<string>(), Some("1"));
Assert.Equal(book.GetOrAddSubBook("bar").TryGet<string>(), Some("2"));

```

#### Threading

*LawBookBuilder* - single thread only

*LawBook* - multithread accessible

#### Recomendation
Use in *Law*  pure functions
Don't use cyclic redudances or use is with care - default 100 cycles of inference is supported.