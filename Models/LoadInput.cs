namespace ALtenExercise.Models
{
    public class LoadInput
    {
        public double Load { get; set; }
        public Fuels fuels { get; set; }
        public List<PowerPlant> PowerPlants { get; set; }
    }
}
