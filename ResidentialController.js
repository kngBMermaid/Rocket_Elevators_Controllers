//-------------------------------------"    Initialize Elevator System     "-------------------------------------

function init_elevatorSystem(numberFloor, numberElevator) {
  var controller = new elevatorController(numberFloor, numberElevator);

  return controller;
}
class column {
  constructor(numberFloor, numberElevator) {
    this.numberFloor = numberFloor;
    this.numberElevator = numberElevator;
    this.elevatorList = [];
    for (let i = 0; i < this.numberElevator; i++) {
      let elevator = new elevator(i, "idle", 1, "up");
      this.elevatorList.push(elevator);
    }
  }
}

class elevator {
  constructor(elevatorNumber, status, elevatorFloor, elevatorDirection) {
    this.elevatorNumber = elevatorNumber;
    this.status = status;
    this.elevatorFloor = elevatorFloor;
    this.elevatorDirection = elevatorDirection;
    this.floorQueue = [];
  }