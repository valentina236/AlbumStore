using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Windows;
using AlbumStore.Models;

namespace AlbumStore.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString = @"Data Source=BTS;Initial Catalog=Album;Integrated Security=True";

        public User AuthenticateUser(string username, string password)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var command = new SqlCommand("SELECT * FROM LoginTable WHERE UserName = @UserName", connection);
                    command.Parameters.AddWithValue("@UserName", username);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string hashedPassword = reader.GetString(2);
                            if (BCrypt.Net.BCrypt.Verify(password, hashedPassword))
                            {
                                return new User
                                {
                                    IdUser = reader.GetInt16(0),
                                    UserName = reader.GetString(1),
                                    UserPassword = null,
                                    RoleName = reader.GetString(3)
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения к базе данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return null;
        }

        public ObservableCollection<Album> GetAlbums()
        {
            var albums = new ObservableCollection<Album>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var command = new SqlCommand(
                    "SELECT * FROM Album", connection);
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    albums.Add(new Album
                    {
                        AlbumCod = reader.GetInt16(0),
                        AlbumName = reader.GetString(1),
                        ReleaseDate = reader.GetDateTime(2),
                        ArtistName = reader.GetString(3),
                        ManufacturerName = reader.GetString(4),
                        Format = GetFormatByCod(reader.GetInt16(5)),
                        Genre = GetGenreByCod(reader.GetInt16(6)),
                        Type = GetTypeByCod(reader.GetInt16(7)),
                        UnitPrice = reader.GetDecimal(8),
                        Photo = reader.GetString(9)
                    });
                }
            }

            return albums;
        }

        public void AddAlbum(Album album)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("AddAlbum", connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@album_name", album.AlbumName);
                command.Parameters.AddWithValue("@release_date", album.ReleaseDate);
                command.Parameters.AddWithValue("@artist_name", album.ArtistName);
                command.Parameters.AddWithValue("@manufacturer_name", album.ManufacturerName);
                command.Parameters.AddWithValue("@cod_format", album.Format.CodFormat);
                command.Parameters.AddWithValue("@cod_genre", album.Genre.CodGenre);
                command.Parameters.AddWithValue("@cod_type", album.Type.CodType);
                command.Parameters.AddWithValue("@unit_price", album.UnitPrice);
                command.Parameters.AddWithValue("@photo", album.Photo);
                command.ExecuteNonQuery();
            }
        }

        public void UpdateAlbum(Album album)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("UpdateAlbum", connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@album_cod", album.AlbumCod);
                command.Parameters.AddWithValue("@album_name", album.AlbumName);
                command.Parameters.AddWithValue("@release_date", album.ReleaseDate);
                command.Parameters.AddWithValue("@artist_name", album.ArtistName);
                command.Parameters.AddWithValue("@manufacturer_name", album.ManufacturerName);
                command.Parameters.AddWithValue("@cod_format", album.Format.CodFormat);
                command.Parameters.AddWithValue("@cod_genre", album.Genre.CodGenre);
                command.Parameters.AddWithValue("@cod_type", album.Type.CodType);
                command.Parameters.AddWithValue("@unit_price", album.UnitPrice);
                command.Parameters.AddWithValue("@photo", album.Photo);
                command.ExecuteNonQuery();
            }
        }

        public void DeleteAlbum(int albumCod)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("DeleteRecordsAlbum", connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@recordID", albumCod);
                command.ExecuteNonQuery();
            }
        }

        public ObservableCollection<Provider> GetProviders()
        {
            var providers = new ObservableCollection<Provider>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT provider_cod, provider_name, addres, phone_fax, rascet_scet FROM Providerr", connection);
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    providers.Add(new Provider
                    {
                        ProviderCod = reader.GetInt16(0),
                        ProviderName = reader.GetString(1),
                        Address = reader.GetString(2),
                        PhoneFax = reader.GetString(3),
                        RascetScet = reader.GetString(4)
                    });
                }
            }

            return providers;
        }

        public ObservableCollection<AlbumFormat> GetFormats()
        {
            var formats = new ObservableCollection<AlbumFormat>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT cod_format, format_name FROM album_format", connection);
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    formats.Add(new AlbumFormat
                    {
                        CodFormat = reader.GetInt16(0),
                        FormatName = reader.GetString(1)
                    });
                }
            }
            return formats;
        }

        public ObservableCollection<AlbumGenre> GetGenres()
        {
            var genres = new ObservableCollection<AlbumGenre>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT cod_genre, genre_name FROM album_genre", connection);
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    genres.Add(new AlbumGenre
                    {
                        CodGenre = reader.GetInt16(0),
                        GenreName = reader.GetString(1)
                    });
                }
            }
            return genres;
        }

        public ObservableCollection<AlbumType> GetTypes()
        {
            var types = new ObservableCollection<AlbumType>();
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT cod_type, typeName FROM album_type", connection);
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    types.Add(new AlbumType
                    {
                        CodType = reader.GetInt16(0),
                        TypeName = reader.GetString(1)
                    });
                }
            }
            return types;
        }


        public void AddProvider(Provider provider)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("AddProvider", connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@provider_name", provider.ProviderName);
                command.Parameters.AddWithValue("@addres", provider.Address);
                command.Parameters.AddWithValue("@phone_fax", provider.PhoneFax);
                command.Parameters.AddWithValue("@rascet_scet", provider.RascetScet);

                command.ExecuteNonQuery();
            }
        }

        public void UpdateProvider(Provider provider)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("UpdateProvider", connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@provider_cod", provider.ProviderCod);
                command.Parameters.AddWithValue("@provider_name", provider.ProviderName);
                command.Parameters.AddWithValue("@addres", provider.Address);
                command.Parameters.AddWithValue("@phone_fax", provider.PhoneFax);
                command.Parameters.AddWithValue("@rascet_scet", provider.RascetScet);

                command.ExecuteNonQuery();
            }
        }

        public void DeleteProvider(int providerCod)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("DeleteRecordsProvider", connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@recordID", providerCod);
                command.ExecuteNonQuery();
            }
        }

        public Provider GetProviderByCod(int providerCod)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT provider_cod, provider_name, addres, phone_fax, rascet_scet FROM Providerr WHERE provider_cod = @providerCod", connection);
                command.Parameters.AddWithValue("@providerCod", providerCod);
                var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    return new Provider
                    {
                        ProviderCod = reader.GetInt16(0),
                        ProviderName = reader.GetString(1),
                        Address = reader.IsDBNull(2) ? null : reader.GetString(2),
                        PhoneFax = reader.IsDBNull(3) ? null : reader.GetString(3),
                        RascetScet = reader.IsDBNull(4) ? null : reader.GetString(4)
                    };
                }
                return null;
            }
        }

        public Album GetAlbumByCod(int albumCod)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT album_cod, album_name, release_date, artist_name, manufacturer_name, cod_format, cod_genre, cod_type, unit_price, photo FROM Album WHERE album_cod = @albumCod", connection);
                command.Parameters.AddWithValue("@albumCod", albumCod);
                var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    var album = new Album
                    {
                        AlbumCod = reader.GetInt16(0),
                        AlbumName = reader.GetString(1),
                        ReleaseDate = reader.GetDateTime(2),
                        ArtistName = reader.GetString(3),
                        ManufacturerName = reader.GetString(4),
                        Format = GetFormatByCod(reader.GetInt16(5)),
                        Genre = GetGenreByCod(reader.GetInt16(6)),
                        Type = GetTypeByCod(reader.GetInt16(7)),
                        UnitPrice = reader.GetDecimal(8),
                        Photo = reader.GetString(9)
                    };

                    return album;
                }
                return null;
            }
        }

        public AlbumFormat GetFormatByCod(int codFormat)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT cod_format, format_name FROM album_format WHERE cod_format = @codFormat", connection);
                command.Parameters.AddWithValue("@codFormat", codFormat);
                var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    return new AlbumFormat
                    {
                        CodFormat = reader.GetInt16(0),
                        FormatName = reader.GetString(1)
                    };
                }
                return null;
            }
        }

        public AlbumGenre GetGenreByCod(int codGenre)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT cod_genre, genre_name FROM album_genre WHERE cod_genre = @codGenre", connection);
                command.Parameters.AddWithValue("@codGenre", codGenre);
                var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    return new AlbumGenre
                    {
                        CodGenre = reader.GetInt16(0),
                        GenreName = reader.GetString(1)
                    };
                }
                return null;
            }
        }

        public AlbumType GetTypeByCod(int codType)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT cod_type, typeName FROM album_type WHERE cod_type = @codType", connection);
                command.Parameters.AddWithValue("@codType", codType);
                var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    return new AlbumType
                    {
                        CodType = reader.GetInt16(0),
                        TypeName = reader.GetString(1)
                    };
                }
                return null;
            }
        }

        public ObservableCollection<TTN> GetTTNs()
        {
            var ttns = new ObservableCollection<TTN>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT num_doc, date_post, provider_cod, album_cod, number_of_albums FROM TTN", connection);
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    ttns.Add(new TTN
                    {
                        NumDoc = reader.GetInt16(0),
                        DatePost = reader.GetDateTime(1),
                        Provider = GetProviderByCod(reader.GetInt16(2)),
                        Album = GetAlbumByCod(reader.GetInt16(3)),
                        NumberOfAlbums = reader.GetInt16(4)
                    });
                }
            }

            return ttns;
        }

        public void AddTTN(TTN ttn)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("AddTTN", connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@date_post", ttn.DatePost);
                command.Parameters.AddWithValue("@provider_cod", ttn.Provider.ProviderCod);
                command.Parameters.AddWithValue("@album_cod", ttn.Album.AlbumCod);
                command.Parameters.AddWithValue("@number_of_albums", ttn.NumberOfAlbums);
                command.ExecuteNonQuery();
            }
        }

        public void UpdateTTN(TTN ttn)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("UpdateTTN", connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@num_doc", ttn.NumDoc);
                command.Parameters.AddWithValue("@provider_cod", ttn.Provider.ProviderCod);
                command.Parameters.AddWithValue("@album_cod", ttn.Album.AlbumCod);
                command.Parameters.AddWithValue("@number_of_albums", ttn.NumberOfAlbums);
                command.ExecuteNonQuery();
            }
        }

        public void DeleteTTN(int numDoc)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("DeleteRecordsTTN", connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@recordID", numDoc);
                command.ExecuteNonQuery();
            }
        }

        public ObservableCollection<Chek> GetCheks()
        {
            var cheks = new ObservableCollection<Chek>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT check_number, date_of_sale, sale_time, album_cod, number_of_albums FROM Chek", connection);
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    cheks.Add(new Chek
                    {
                        CheckNumber = reader.GetInt16(0),
                        DateOfSale = reader.GetDateTime(1),
                        SaleTime = reader.GetTimeSpan(2),
                        Album = GetAlbumByCod(reader.GetInt16(3)),
                        NumberOfAlbums = reader.GetInt16(4)
                    });
                }
            }

            return cheks;
        }

        public void AddChek(Chek chek)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("AddCheck", connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@album_cod", chek.Album.AlbumCod);
                command.Parameters.AddWithValue("@number_of_albums", chek.NumberOfAlbums);
                command.ExecuteNonQuery();
            }
        }

        public void UpdateChek(Chek chek)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("UpdateChek", connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@check_number", chek.CheckNumber);
                command.Parameters.AddWithValue("@album_cod", chek.Album.AlbumCod);
                command.Parameters.AddWithValue("@number_of_albums", chek.NumberOfAlbums);
                command.ExecuteNonQuery();
            }
        }

        public void DeleteChek(int checkNumber)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("DeleteRecordsCheck", connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@recordID", checkNumber);
                command.ExecuteNonQuery();
            }
        }

        public ObservableCollection<JurnalUceta> GetJurnalUceta()
        {
            var jurnalUceta = new ObservableCollection<JurnalUceta>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT month_name, album_cod, NumbeOfDeliveredAlbums, NumberOfAlbumsSold FROM Jurnal_uceta", connection);
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    jurnalUceta.Add(new JurnalUceta
                    {
                        MonthName = reader.GetString(0),
                        Album = GetAlbumByCod(reader.GetInt16(1)),
                        NumbeOfDeliveredAlbums = reader.GetInt16(2),
                        NumberOfAlbumsSold = reader.GetInt16(3)
                    });
                }
            }

            return jurnalUceta;
        }

        public void AddJurnalUceta(JurnalUceta jurnal)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("AddJurnalInfo", connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@month_name", jurnal.MonthName);
                command.Parameters.AddWithValue("@album_cod", jurnal.Album.AlbumCod);
                command.Parameters.AddWithValue("@NumbeOfDeliveredAlbums", jurnal.NumbeOfDeliveredAlbums);
                command.Parameters.AddWithValue("@NumberOfAlbumsSold", jurnal.NumberOfAlbumsSold);

                command.ExecuteNonQuery();
            }
        }

        public void UpdateJurnalUceta(JurnalUceta jurnal)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("UpdateJurnalUceta", connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@month_name", jurnal.MonthName);
                command.Parameters.AddWithValue("@album_cod", jurnal.Album.AlbumCod);
                command.Parameters.AddWithValue("@NumbeOfDeliveredAlbums", jurnal.NumbeOfDeliveredAlbums);
                command.Parameters.AddWithValue("@NumberOfAlbumsSold", jurnal.NumberOfAlbumsSold);

                command.ExecuteNonQuery();
            }
        }

        public void DeleteJurnalUceta(string monthName, string albumName)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("DeleteRecordsUcet", connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@month", monthName);
                command.Parameters.AddWithValue("@name", albumName);
                command.ExecuteNonQuery();
            }
        }

        public ObservableCollection<User> GetUsers()
        {
            var users = new ObservableCollection<User>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT IdUser, UserName, UserPassword, RoleName FROM LoginTable", connection);
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    users.Add(new User
                    {
                        IdUser = reader.GetInt16(0),
                        UserName = reader.GetString(1),
                        UserPassword = reader.GetString(2),
                        RoleName = reader.GetString(3)
                    });
                }
            }

            return users;
        }

        public void AddUser(User user)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("AddUser", connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@UserName", user.UserName);
                command.Parameters.AddWithValue("@UserPassword", user.UserPassword);
                command.Parameters.AddWithValue("@RoleName", user.RoleName);

                command.ExecuteNonQuery();
            }
        }

        public void UpdateUser(User user)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("UpdateUserInfo", connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@IdUser", user.IdUser);
                command.Parameters.AddWithValue("@UserName", user.UserName);
                command.Parameters.AddWithValue("@UserPassword", user.UserPassword);
                command.Parameters.AddWithValue("@RoleName", user.RoleName);

                command.ExecuteNonQuery();
            }
        }

        public void DeleteUser(int userId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("DeleteUser", connection)
                {
                    CommandType = System.Data.CommandType.StoredProcedure
                };
                command.Parameters.AddWithValue("@UserID", userId);
                command.ExecuteNonQuery();
            }
        }

        public void CreateBackupDB(string FilePath)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "BACKUP DATABASE Album TO DISK = N'" + FilePath + "' WITH NOFORMAT, NOINIT, NAME = N'Album-Full Database Backup', SKIP, NOREWIND, NOUNLOAD";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void RestoreBackupDB(string FilePath)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                bool isDatabaseInUse = IsDatabaseInUse(connection, "Album");
                if (isDatabaseInUse) DisconnectDatabase(connection, "Album");
                string query = "USE master RESTORE DATABASE Album FROM DISK = N'" + FilePath + "' WITH REPLACE, FILE = 1,  NOUNLOAD";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private bool IsDatabaseInUse(SqlConnection connection, string databaseName)
        {
            string query = $"SELECT COUNT(*) FROM sys.dm_exec_sessions WHERE database_id = DB_ID('{databaseName}')";
            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                int sessionCount = (int)cmd.ExecuteScalar();
                return sessionCount > 0;
            }
        }

        private void DisconnectDatabase(SqlConnection connection, string databaseName)
        {
            string query = $"ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE";
            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                cmd.ExecuteNonQuery();
            }
        }
    }
}