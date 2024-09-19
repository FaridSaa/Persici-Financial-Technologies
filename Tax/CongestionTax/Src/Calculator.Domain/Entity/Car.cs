namespace Calculator.Domain.Entity
{
    public class Car : IVehicle
    {
        public VehicleType GetVehicleType() => VehicleType.Car;
    }
}
