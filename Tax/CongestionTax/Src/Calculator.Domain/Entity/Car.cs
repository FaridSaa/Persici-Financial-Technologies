namespace Calculator.Domain.Entity
{
    public class Car : IVehicle
    {
        public VehicleTypeEnum GetVehicleType() => VehicleTypeEnum.Car;
    }
}
