using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dbosoft.YaNco;
using Dbosoft.YaNco.TypeMapping;
using Microsoft.Extensions.Configuration;

var configurationBuilder = new ConfigurationBuilder();
var config = configurationBuilder.Build();

var settings = new Dictionary<string, string>
{
    {"ashost", "10.45.4.163"},
    {"sysnr", "01"},
    {"client", "200"},
    {"user", "USU_INTEGRAC"},
    {"passwd","Rocio*25"},
    {"lang", "EN"}
};

var connectionBuilder = new ConnectionBuilder(settings);
var connFunc = connectionBuilder.Build();

using (var context = new RfcContext(connFunc))
{
    var result = await context.CallFunction("ZMM_FM_MIGR_MATERIAL_DATA",
        Input: f => f
            .SetField("I_BUKRS", "1700")
            .SetField("I_FECMOD_I", DateTime.ParseExact("14.04.2025", "dd.MM.yyyy", null))   //modificado fecha inicio
            .SetField("I_FECMOD_F", DateTime.ParseExact("15.04.2025", "dd.MM.yyyy", null)),  //modificado fecha fin
        Output: f => f
           .MapTable("T_DATA_MAT", s =>
                          from colum1 in s.GetField<string>("MATNR")    // CHAR
                          from colum2 in s.GetField<string>("MAKTX")    // CHAR
                          from colum3 in s.GetField<string>("WERKS")    // CHAR
                          from colum4 in s.GetField<string>("LGORT")    // CHAR
                          from colum5 in s.GetField<int>("MEINS")    // UNIT
                          from colum6 in s.GetField<int>("GEWEI")    // UNIT
                          from colum7 in s.GetField<decimal>("MENGE")   // QUAN
                          from colum8 in s.GetField<string>("MATKL")    // CHAR
                          from colum9 in s.GetField<string>("MTART")    // CHAR
                          from colum10 in s.GetField<string>("SPART")   // CHAR
                          from colum11 in s.GetField<string>("XCHPF")   // CHAR
                          from colum12 in s.GetField<string>("XCHPF_C")  // CHAR
                          from colum13 in s.GetField<decimal>("PRICE_UN") // CURR
                          from colum14 in s.GetField<string>("WAERS")
                          select (colum1, colum2, colum3, colum4, colum5, colum6, colum7, colum8, colum9, colum10, colum11, colum12, colum13, colum14)))
        .Match(
            r =>
            {
                foreach (var (colum1, colum2, colum3, colum4, colum5, colum6, colum7, colum8, colum9, colum10, colum11, colum12, colum13, colum14) in r)
                {
                    Console.WriteLine($"{colum1}\t{colum2}\t{colum3}");
                    InsertDataIntoSqlServer(colum1, colum2, colum3, colum4,colum5, colum6, colum7, colum8, colum9, colum10, colum11, colum12, colum13, colum14);
                }
            },
            l => Console.WriteLine($"Error: {l.Message}"));
}

void InsertDataIntoSqlServer(string matnr, string maktx, string werks, string lgort, int meins, int gewei, decimal menge, string matkl, string mtart, string spart, string xchpf, string xchpf_c, decimal price_un, string waers)
{
    string connectionString = "Server=10.45.0.217;Database=BDGenesysAvi_QAS;User Id=u_wost;Password=$w0st#sql+;";

    using (SqlConnection connection = new SqlConnection(connectionString))
    {
        connection.Open();
        string query = "INSERT INTO Temp_Materiales_Detalle (MATNR, MAKTX, WERKS, LGORT, MEINS, GEWEI, MENGE, MATKL, MTART, SPART, XCHPF, XCHPF_C, PRICE_UN, WAERS) VALUES (@matnr, @maktx, @werks, @lgort, @meins, @gewei, @menge, @matkl, @mtart, @spart, @xchpf, @xchpf_c, @price_un, @waers);";

        using (SqlCommand command = new SqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@matnr", matnr);
            command.Parameters.AddWithValue("@maktx", maktx);
            command.Parameters.AddWithValue("@werks", werks);
            command.Parameters.AddWithValue("@lgort", lgort);
            command.Parameters.AddWithValue("@meins", meins);
            command.Parameters.AddWithValue("@gewei", gewei);
            command.Parameters.AddWithValue("@menge", menge);
            command.Parameters.AddWithValue("@matkl", matkl);
            command.Parameters.AddWithValue("@mtart", mtart);
            command.Parameters.AddWithValue("@spart", spart);
            command.Parameters.AddWithValue("@xchpf", xchpf);
            command.Parameters.AddWithValue("@xchpf_c", xchpf_c);
            command.Parameters.AddWithValue("@price_un", price_un);
            command.Parameters.AddWithValue("@waers", waers);

            try
            {
                command.ExecuteNonQuery();
                Console.WriteLine("Data inserted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting data: {ex.Message}");
            }
        }
    }
}