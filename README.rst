============
Crate.Client
============

.. image:: https://ci.appveyor.com/api/projects/status/tpcf77kxwe9knukd/branch/master?svg=true
    :target: https://ci.appveyor.com/project/SherzodMutalov/crate-net
    :alt: appveyor


Crate.Net is a Mono/.NET client driver implementing the ADO.NET interface
for `Crate <https://crate.io>`_

::


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


Things missing
==============

Currently this is just a prototype. Things that are missing are:

* DataAdapter class
* an EntityFramework Provider
