============
Crate.Client
============

.. image:: https://ci.appveyor.com/api/projects/status/tpcf77kxwe9knukd/branch/master?svg=true
    :target: https://ci.appveyor.com/project/SherzodMutalov/crate-net
    :alt: appveyor


Crate.Net is a Mono/.NET client driver implementing the ADO.NET interface
for `Crate <https://crate.io>`_

::

    using Crate.Net.Client;

    using (var conn = new CrateConnection()) {
        conn.Open();
        using (var cmd = new CrateCommand("select name from sys.cluster", conn)) {
            var reader = cmd.ExecuteReader();
            reader.Read();
            string clusterName = reader.GetString(0);
        }
    }

Things missing
==============

Currently this is just a prototype. Things that are missing are:

* DataAdapter class
* an EntityFramework Provider
