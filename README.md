Crate.Net.Client
================
Crate&#46;Net is a Mono/.NET client driver implementing the ADO&#46;NET interface for [Crate](http://crate.io)

```csharp
using (var conn = new CrateConnection("Server=localhost;Port=4200"))
{
    conn.Open();

    using (var cmd = conn.CreateCommand())
    {
        cmd.CommandText = "select name from sys.cluster";

        using (var reader = cmd.ExecuteReader())
        {
            reader.Read();
            var clusterName = reader.GetString(0);
            Assert.AreEqual(clusterName, "crate");
        }
    }
}
```
Things missing
--------------
Currently this is just a prototype. Things that are missing are:
- DataAdapter class
- an EntityFramework Provider
