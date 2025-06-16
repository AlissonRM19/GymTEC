using GymTec.Api.Models;

public class TipoPlanilla
{
    public int Id { get; set; }
    public string Descripcion { get; set; } = null!;

    public ICollection<User> Users { get; set; } = new List<User>();
}

