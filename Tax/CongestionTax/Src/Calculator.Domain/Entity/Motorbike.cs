namespace Calculator.Domain.Entity
{
    public class Motorbike : IVehicle
    {
        public VehicleTypeEnum GetVehicleType() => VehicleTypeEnum.Motorcycles;
    }
}
