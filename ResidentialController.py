#-------------------------------------"    Allows the creation of pauses when running the code     "-------------------------------------

import time

#-------------------------------------"    Initialize Elevator System     "-------------------------------------

class elevatorController:
    def __init__(self, numberFloor, numberElevator):
        self.numberFloor = numberFloor
        self.numberElevator = numberElevator
        self.column = Column(numberFloor, numberElevator)
        print("Elevator System Initialized")

#-------------------------------------"  Elevator Call   "-------------------------------------

    def callElevator(self, floorNumber, direction):
        time.sleep(1)
        print("==================================")
        print("Elevator Requested on Floor : ", floorNumber)
        time.sleep(1)
        print("==================================")
        print("Call Elevator Button : Active")
        time.sleep(1)
        elevator = self.selectOptimalElevator(floorNumber, direction)
        elevator.addtoFloorQueue(floorNumber)
        return elevator

#-------------------------------------"  Floor Call   "-------------------------------------

    def callFloor(self, elevator, requestedFloor):
        time.sleep(1)
        print("==================================")
        print("Requested Destination Floor : ", requestedFloor)
        time.sleep(1)
        print("==================================")
        print("Request Floor Button : Active")
        time.sleep(1)
        elevator.addtoFloorQueue(requestedFloor)

#-------------------------------------"    Selecting the Optimal Elevator    "-------------------------------------

    def selectOptimalElevator(self, floorNumber, direction):
        optimalElevator = None
        smallestDistance = 1000
        for elevator in (self.column.elevatorList):
            if (floorNumber == elevator.elevatorFloor and (elevator.status == "stopped" or elevator.status == "idle" or elevator.status == "moving")):
                return elevator
            else:
                distance = abs(floorNumber - elevator.elevatorFloor)
                if smallestDistance > distance:
                    smallestDistance = distance
                    optimalElevator = elevator

                elif elevator.direction == direction:
                    optimalElevator = elevator

        return optimalElevator

#-------------------------------------"    Initialize Elevator    "-------------------------------------

class Elevator:
    def __init__(self, elevatorNumber, status, elevatorFloor, elevatordirection):
        self.elevatorNumber = elevatorNumber
        self.status = status
        self.elevatorFloor = elevatorFloor
        self.elevatordirection = elevatordirection
        self.floorQueue = []

#-------------------------------------"    Pushing and Sorting the Requested Elevator in the Floor Queue  "-------------------------------------

    def addtoFloorQueue(self, requestedFloor):
        self.floorQueue.append(requestedFloor)
        self.sortFloorQueue()
        self.moveElevator(requestedFloor)

    def sortFloorQueue(self):
        if self.elevatordirection == "up":
            self.floorQueue.sort()
        elif self.elevatordirection == "down":
            self.floorQueue.sort()
            self.floorQueue.reverse()
        return self.floorQueue

#-------------------------------------"    Elevator Movements    "-------------------------------------

    def moveElevator(self, requestedFloor):
        while (len(self.floorQueue) > 0):
            if ((requestedFloor == self.elevatorFloor)):
                self.openDoors()
                self.status = "moving"

                self.floorQueue.pop()
            elif (requestedFloor < self.elevatorFloor):

                self.status = "moving"
                print("==================================")
                print("Elevator", self.elevatorNumber, self.status)
                print("==================================")
                self.direction = "down"
                self.moveDown(requestedFloor)
                self.status = "stopped"
                print("==================================")
                print("Elevator", self.elevatorNumber, self.status)
                print("==================================")
                self.openDoors()
                self.floorQueue.pop()
            elif (requestedFloor > self.elevatorFloor):

                time.sleep(1)
                self.status = "moving"
                print("==================================")
                print("Elevator", self.elevatorNumber, self.status)
                print("==================================")
                self.direction = "up"
                self.moveUp(requestedFloor)
                self.status = "stopped"
                print("==================================")
                print("Elevator", self.elevatorNumber, self.status)
                print("==================================")

                self.openDoors()

                self.floorQueue.pop()

        if self.floorQueue == 0:
            self.status = "idle"

#-------------------------------------"  Doors Status  "-------------------------------------

    def openDoors(self):
        time.sleep(1)
        print("Open Doors")
        print("==================================")
        print("Button : Innactive")
        time.sleep(1)
        print("==================================")
        time.sleep(1)
        self.CloseDoors()

    def CloseDoors(self):
        print("Close Doors")
        time.sleep(1)

#-------------------------------------"    Moving Up    "-------------------------------------

    def moveUp(self, requestedFloor):
        print("Floor : ", self.elevatorFloor)
        time.sleep(1)
        while(self.elevatorFloor != requestedFloor):
            self.elevatorFloor += 1
            print("Floor : ", self.elevatorFloor)
            time.sleep(1)

#-------------------------------------"    Moving Down    "-------------------------------------

    def moveDown(self, requestedFloor):
        print("Floor : ", self.elevatorFloor)
        time.sleep(1)
        while(self.elevatorFloor != requestedFloor):
            self.elevatorFloor -= 1
            print("Floor : ", self.elevatorFloor)

            time.sleep(1)

class elevatorCallButton:
    def __init__(self, floorNumber, direction):
        self.floorNumber = floorNumber
        self.direction = direction
        self.light = False


class floorCallButton:
    def __init__(self, requestedFloor):
        self.requestedFloor = requestedFloor


class Column:
    def __init__(self, numberFloor, numberElevator):
        self.numberFloor = numberFloor
        self.numberElevator = numberElevator
        self.elevatorList = []
        for i in range(numberElevator):
            elevator = Elevator(i, "idle", 1, "up")
            self.elevatorList.append(elevator)



def testScenario1():

    controller = elevatorController(10, 2)

    controller.column.elevatorList[0].elevatorFloor = 2
    controller.column.elevatorList[0].status = "moving"
    controller.column.elevatorList[0].elevatordirection = "down"

    controller.column.elevatorList[1].elevatorFloor = 6
    controller.column.elevatorList[1].status = "moving"
    controller.column.elevatorList[1].elevatordirection = "down"

    elevator = controller.callElevator(5, "up")
    controller.callFloor(elevator, 7)

    print("==================================")
    print("Scenario 1 Ended")
    print("==================================")

def testScenario2():


    controller = elevatorController(10, 2)

    controller.column.elevatorList[0].elevatorFloor = 10
    controller.column.elevatorList[0].status = "moving"
    controller.column.elevatorList[0].elevatordirection = "down"

    controller.column.elevatorList[1].elevatorFloor = 3
    controller.column.elevatorList[1].status = "moving"
    controller.column.elevatorList[1].elevatordirection = "down"


    elevator = controller.callElevator(1, "up")
    controller.callFloor(elevator, 6)
    elevator = controller.callElevator(3, "up")
    controller.callFloor(elevator, 5)
    elevator = controller.callElevator(9, "down")
    controller.callFloor(elevator, 2)

    print("==================================")
    print("Scenario 2 Ended")
    print("==================================")
    
def testScenario3():

    controller = elevatorController(10, 2)

    controller.column.elevatorList[0].elevatorFloor = 10
    controller.column.elevatorList[0].status = "moving"
    controller.column.elevatorList[0].elevatordirection = "down"

    controller.column.elevatorList[1].elevatorFloor = 3
    controller.column.elevatorList[1].status = "moving"
    controller.column.elevatorList[1].elevatordirection = "down"

    print(controller.column.elevatorList)
    elevator = controller.callElevator(10, "down")
    controller.callFloor(elevator, 3)
    elevator = controller.callElevator(3, "down")
    controller.callFloor(elevator, 2)

    print("==================================")
    print("Scenario 3 Ended")
    print("==================================")

testScenario1()
#testScenario2()
#testScenario3()