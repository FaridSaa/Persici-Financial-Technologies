namespace Calculator.Domain.Entity
{
    public class Motorbike : IVehicle
    {
        public VehicleType GetVehicleType() => VehicleType.Motorcycles;
    }
}
