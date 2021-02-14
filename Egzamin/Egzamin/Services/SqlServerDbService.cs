using Egzamin.DTOs.Responses;
using Egzamin.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Egzamin.Services
{
    public class SqlServerDbService : IClinicDbService
    {
        public GetMedicamentResponse GetMedicament(string id)
        {
            var resultOfGet = new GetMedicamentResponse();

            using (SqlConnection con = new SqlConnection(Program.GetConnectionString()))
            using (SqlCommand com = new SqlCommand())
            {
                com.Connection = con;
                com.Parameters.AddWithValue("id", id);

                com.CommandText = 
                    "SELECT Medicament.IdMedicament, Medicament.Name, Medicament.Description, Medicament.Type, " +
                    "Prescription.IdPrescription, Prescription.Date, Prescription.DueDate, Prescription.IdPatient, IdDoctor " +
                    "FROM Medicament " +
                    "INNER JOIN Prescription_Medicament ON Prescription_Medicament.IdMedicament=Medicament.IdMedicament " +
                    "INNER JOIN Prescription ON Prescription_Medicament.IdPrescription=Prescription.IdPrescription " +
                    "WHERE Medicament.IdMedicament=@id " +
                    "ORDER BY Prescription.Date DESC";

                con.Open();
                SqlDataReader dr = com.ExecuteReader();
                bool firstRead = true;
                while (dr.Read())
                {
                    if (firstRead)
                    {
                        resultOfGet.Id = (int)dr["IdMedicament"];
                        resultOfGet.Name = dr["Name"].ToString();
                        resultOfGet.Description = dr["Description"].ToString();
                        resultOfGet.Type = dr["Type"].ToString();
                        firstRead = false;
                    }
                    var presc = new Prescription
                    {
                        Id = (int)dr["IdPrescription"],
                        Date = (DateTime)dr["Date"],
                        DueDate = (DateTime)dr["DueDate"],
                        IdPatient = (int)dr["IdPatient"],
                        IdDoctor = (int)dr["IdDoctor"]
                    };

                    resultOfGet.Prescriptions.Add(presc);
                }
            }

            return resultOfGet;
        }

        public DeletePatientResponse DeletePatient(string id)
        {
            var resultOfDelete = new DeletePatientResponse();
            bool succeeded = false;

            using (var con = new SqlConnection(Program.GetConnectionString()))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                con.Open();
                var tran = con.BeginTransaction();

                try
                {
                    com.CommandText = "SELECT * FROM Patient WHERE id=@id";
                    com.Parameters.AddWithValue("id", id);
                    com.Transaction = tran;

                    var dr = com.ExecuteReader();
                    if (!dr.Read())
                    {
                        dr.Close();
                        tran.Rollback();
                        throw new Exception("Nie udało się odnaleźć pacjenta o wskazanym id.");
                    }
                    else
                    {
                        resultOfDelete.Id = (int)dr["IdPatient"];
                        resultOfDelete.FirstName = dr["FirstName"].ToString();
                        resultOfDelete.LastName = dr["LastName"].ToString();
                        resultOfDelete.Birthdate = (DateTime)dr["Birthdate"];
                    }
                    dr.Close();

                    com.CommandText = "SELECT IdPrescription FROM Prescription " +
                        "INNER JOIN Patient ON Prescription.IdPatient=Patient.IdPatient " +
                        "WHERE Prescription.IdPatient=@id";
                    com.Parameters.AddWithValue("id", id);
                    com.Transaction = tran;

                    dr = com.ExecuteReader();
                    var listOfPrescriptionsContainingPatientWithId = new List<Int32>();
                    while (dr.Read())
                    {
                        int prescId = (int)dr["IdPrescription"];
                        listOfPrescriptionsContainingPatientWithId.Add(prescId);
                    }
                    dr.Close();

                    if (listOfPrescriptionsContainingPatientWithId.Count != 0)
                    {
                        bool wasBroken = false;
                        int counter = 0;
                        while (counter < listOfPrescriptionsContainingPatientWithId.Count - 1)
                        {
                            int prescId = listOfPrescriptionsContainingPatientWithId[counter];
                            com.CommandText = "DELETE FROM Prescription_Medicament " +
                                "WHERE Prescription_Medicament.IdPrescription=@id";
                            com.Parameters.AddWithValue("id", prescId);
                            com.Transaction = tran;

                            int rowsAffected = com.ExecuteNonQuery();

                            if (rowsAffected <= 0)
                            {
                                wasBroken = true;
                                break;
                            }

                            counter++;
                        }

                        if (wasBroken)
                        {
                            tran.Rollback();
                            throw new Exception("Wystąpił błąd w czasie usuwania powiązanych z lekami recept dla podanego pacjenta.");
                        }
                        else
                        {
                            counter = 0;
                            while (counter < listOfPrescriptionsContainingPatientWithId.Count - 1)
                            {
                                int prescId = listOfPrescriptionsContainingPatientWithId[counter];
                                com.CommandText = "DELETE FROM Prescription " +
                                    "WHERE Prescription.IdPrescription=@id";
                                com.Parameters.AddWithValue("id", prescId);
                                com.Transaction = tran;

                                int rowsAffected = com.ExecuteNonQuery();

                                if (rowsAffected != 1)
                                {
                                    wasBroken = true;
                                    break;
                                }

                                counter++;
                            }
                        }

                        if (wasBroken)
                        {
                            tran.Rollback();
                            throw new Exception("Wystąpił błąd w czasie usuwania recept dla podanego pacjenta.");
                        }
                    }

                    com.CommandText = "DELETE FROM Patient " +
                                "WHERE Patient.IdPatient=@id";
                    com.Parameters.AddWithValue("id", id);
                    com.Transaction = tran;

                    int rowAffected = com.ExecuteNonQuery();

                    if (rowAffected != 1)
                    {
                        tran.Rollback();
                        throw new Exception("Wystąpił błąd w czasie usuwania pacjenta.");
                    }

                    tran.Commit();
                    succeeded = true;

                } catch (SqlException exception)
                {
                    throw new Exception(exception.ToString());
                }
            }

            if (!succeeded)
            {
                return null;
            }

            return resultOfDelete;
        }
    }
}
