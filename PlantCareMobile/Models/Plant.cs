using SQLite;

namespace PlantCareMobile.Models
{
    public class Plant
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string NombreCientifico { get; set; }
        public string NombreComun { get; set; }
        public double PuntajeIdentificacion { get; set; }
        public string RutaImagen { get; set; }
        public DateTime FechaGuardado { get; set; }
    }
}
