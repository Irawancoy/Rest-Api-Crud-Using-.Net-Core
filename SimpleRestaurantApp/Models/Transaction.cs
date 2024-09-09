using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SimpleRestaurantApp.Models;

public partial class Transaction
{
    public int Id { get; set; }

    public int Idcustomers { get; set; }

    public int Idfood { get; set; }

    public int Quantity { get; set; }

    public int Totalprice { get; set; }

    public DateTime Transactiondate { get; set; }

    // Properti navigasi untuk pelanggan terkait, diabaikan saat serialisasi JSON
    [JsonIgnore]
    public virtual Customer? IdcustomersNavigation { get; set; }

    // Properti navigasi untuk makanan terkait, diabaikan saat serialisasi JSON
    [JsonIgnore]
    public virtual Food? IdfoodNavigation { get; set; }

}
