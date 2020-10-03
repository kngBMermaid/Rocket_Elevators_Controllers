//-------------------------------------"    Equivalent of Importing Time in Python (Found on StackOverFlow)     "-------------------------------------

function sleep(milliseconds) {
  var start = new Date().getTime();
  for (var i = 0; i < 1e7; i++) {
    if (new Date().getTime() - start > milliseconds) {
      break;
    }
  }
}

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
      let elevator = new Elevator(i, "idle", 1, "up");
      this.elevatorList.push(elevator);
    }
  }
}

class Elevator {
  constructor(elevatorNumber, status, elevatorFloor, elevatorDirection) {
    this.elevatorNumber = elevatorNumber;
    this.status = status;
    this.elevatorFloor = elevatorFloor;
    this.elevatorDirection = elevatorDirection;
    this.floorQueue = [];
  }

  //-------------------------------------"    Pushing and Sorting the Requested Elevator in the Floor Queue    "-------------------------------------

  addtoFloorQueue(requestedFloor) {
    this.floorQueue.push(requestedFloor);
    this.sortFloorQueue();
    this.moveElevator(requestedFloor);
  }

  sortFloorQueue() {
    if (this.elevatorDirection === "up") {
      this.floorQueue.sort();
    } else if (this.elevatorDirection === "down") {
      this.floorQueue.sort();
      this.floorQueue.reverse();
    }
    return this.floorQueue;
  }

  //-------------------------------------"   Requesting the Elevator Movement   "-------------------------------------

  moveElevator(requestedFloor) {
    while (this.floorQueue > 0) {
      if (requestedFloor === this.elevatorFloor) {
        this.openDoors();
        this.status = "moving";
        this.floorQueue.shift();
      } else if (requestedFloor < this.elevatorFloor) {
        this.status = "moving";
        console.log("=================================");
        console.log("Elevator " + this.elevatorNumber, this.status);
        sleep(1000);
        console.log("=================================");
        this.Direction = "down";
        this.moveDown(requestedFloor);
        this.status = "stopped";
        console.log("=================================");
        console.log("Elevator " + this.elevatorNumber, this.status);
        console.log("=================================");
        this.openDoors();
        this.floorQueue.shift();
      } else if (requestedFloor > this.elevatorFloor) {
        this.status = "moving";
        sleep(1000);
        console.log("=================================");
        console.log("Elevator " + this.elevatorNumber, this.status);
        sleep(1000);
        console.log("=================================");
        this.Direction = "up";
        this.moveUp(requestedFloor);
        this.status = "stopped";
        sleep(1000);
        console.log("=================================");
        console.log("Elevator " + this.elevatorNumber, this.status);
        sleep(1000);
        console.log("=================================");
        this.openDoors();
        this.floorQueue.shift();
      }
    }
    if (this.floorQueue === 0) {
      this.status = "idle";
    }
  }

  //-------------------------------------"  Floor Call Button  "-------------------------------------

  floorCallButton(requestedFloor) {
    this.requestedFloor = requestedFloor;
  }

  //-------------------------------------"  Elevator Call Button  "-------------------------------------

  elevatorCallButton(floorNumber, Direction) {
    this.floorNumber = floorNumber;
    this.Direction = Direction;
  }

  //-------------------------------------"  Doors Status  "-------------------------------------

  openDoors() {
    sleep(1000);
    console.log("Open Doors");
    console.log("=================================");
    sleep(1000);
    console.log("Button Status : Innactive");
    console.log("=================================");
    this.closeDoors();
  }

  closeDoors() {
    sleep(1000);
    console.log("Close Doors");
    sleep(1000);
  }

  //-------------------------------------"    Moving Up    "-------------------------------------

  moveUp(requestedFloor) {
    console.log("Floor : " + this.elevatorFloor);
    sleep(1000);
    while (this.elevatorFloor !== requestedFloor) {
      this.elevatorFloor += 1;
      console.log("Floor : " + this.elevatorFloor);
      sleep(1000);
    }
  }

  //-------------------------------------"    Moving Down    "-------------------------------------

  moveDown(requestedFloor) {
    console.log("Floor : " + this.elevatorFloor);
    sleep(1000);
    while (this.elevatorFloor !== requestedFloor) {
      this.elevatorFloor -= 1;
      console.log("Floor : " + this.elevatorFloor);
      sleep(1000);
    }
  }
}

class elevatorController {
  constructor(numberFloor, numberElevator) {
    this.numberFloor = numberFloor;
    this.numberElevator = numberElevator;
    this.column = new column(numberFloor, numberElevator);
    console.log("Elevator System Initialized");
    sleep(1000);
  }

  //-------------------------------------"  Elevator Call   "-------------------------------------

  callElevator(floorNumber, Direction) {
    console.log("=================================");
    console.log("Elevator Requested on Floor : ", floorNumber);
    sleep(1000);
    console.log("=================================");
    console.log("Call Elevator Button : Active");
    sleep(1000);

    let elevator = this.selectOptimalElevator(floorNumber, Direction);
    elevator.addtoFloorQueue(floorNumber);
    return elevator;
  }

  //-------------------------------------"  Floor Call   "-------------------------------------

  callFloor(elevator, requestedFloor) {
    console.log("=================================");
    console.log("Requested Destination Floor : ", requestedFloor);
    sleep(1000);
    console.log("=================================");
    console.log("Request Floor Button : Active");
    elevator.addtoFloorQueue(requestedFloor);
    sleep(1000);
  }

  //-------------------------------------"    Selecting the Optimal Elevator    "-------------------------------------

  selectOptimalElevator(floorNumber, Direction) {
    let optimalElevator = null;
    let smallestDistance = 1000;
    for (let i = 0; i < this.column.elevatorList.length; i++) {
      let elevator = this.column.elevatorList[i];

      if (
        floorNumber === elevator.elevatorFloor &&
        (elevator.status === "stopped" ||
          elevator.status === "idle" ||
          elevator.status === "moving")
      ) {
        return elevator;
      } else {
        let distance = Math.abs(floorNumber - elevator.elevatorFloor);
        if (smallestDistance > distance) {
          smallestDistance = distance;
          optimalElevator = elevator;

          if (elevator.Direction === Direction) {
            optimalElevator = elevator;
          }
        }
      }
    }
    return optimalElevator;
  }
}



function testScenario1() {
  var controller = init_elevatorSystem(10, 2);

   controller.column.elevatorList[0].elevatorFloor = 2;
   controller.column.elevatorList[0].status = "moving";
   controller.column.elevatorList[0].elevatorDirection = "down";
   controller.column.elevatorList[1].elevatorFloor = 6;
   controller.column.elevatorList[1].status = "moving";
   controller.column.elevatorList[1].elevatorDirection = "down";

  var elevator = controller.callElevator(5, "up");
   controller.callFloor(elevator, 7);
   console.log("==============================");
   console.log("Scenario 1 Ended");
   console.log("==============================");
}

function testScenario2() {
  var controller = init_elevatorSystem(10, 2);

  controller.column.elevatorList[0].elevatorFloor = 10;
  controller.column.elevatorList[0].status = "moving";
  controller.column.elevatorList[0].elevatorDirection = "down";
  controller.column.elevatorList[1].elevatorFloor = 3;
  controller.column.elevatorList[1].status = "moving";
  controller.column.elevatorList[1].elevatorDirection = "down";

  var elevator = controller.callElevator(1, "up");
  controller.callFloor(elevator, 6);
  elevator = controller.callElevator(3, "up");
  controller.callFloor(elevator, 5);
  elevator = controller.callElevator(9, "down");
  controller.callFloor(elevator, 2);
  
  console.log("==============================");
  console.log("Scenario 2 Ended");
  console.log("==============================");
}

function testScenario3() {
  var controller = init_elevatorSystem(10, 2);

  controller.column.elevatorList[0].elevatorFloor = 10;
  controller.column.elevatorList[0].status = "moving";
  controller.column.elevatorList[0].elevatorDirection = "down";
  controller.column.elevatorList[1].elevatorFloor = 3;
  controller.column.elevatorList[1].status = "moving";
  controller.column.elevatorList[1].elevatorDirection = "down";

  console.log(controller.column.elevatorList)
  var elevator = controller.callElevator(10, "down"); 
  controller.callFloor(elevator, 3);

  elevator = controller.callElevator(3, "down");
  controller.callFloor(elevator, 2);

  console.log("==============================");
  console.log("Scenario 3 Ended")
  console.log("==============================");
}

testScenario1();
//testScenario2();
//testScenario3();