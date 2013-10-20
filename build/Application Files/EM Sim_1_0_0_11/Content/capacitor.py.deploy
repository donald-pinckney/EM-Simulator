# Setup iterator for custom increments - from http://stackoverflow.com/questions/4930404/how-do-get-more-control-over-loop-increments-in-python
def drange2(start, stop, step):
    numelements = int((stop-start)/float(step))
    for i in range(numelements+1):
            yield start + i*step


autogen(False)

# Set constants for x, z, and y bounds
minX = -20
maxX = 20
minZ = -20
maxZ = 20
topY = 30
botY = -30

# Set constants for x and z gaps
xGap = 2
zGap = 2

# Set constant charge per plate
totalChargePerPlate = 1.0

# Calculate the number of charges per plate
numXRows = ((maxX-minX)/xGap)+1
numZRows = ((maxZ-minZ)/zGap)+1
pointsPerPlate = numXRows * numZRows

# Calculate the charge per point charge
unitCharge = totalChargePerPlate/pointsPerPlate


# Make positive top plate of capacitor
for x in drange2(minX, maxX, xGap):
	for z in drange2(minZ, maxZ, zGap):
		addCharge(x, topY, z, unitCharge)

# Make negative bottom plate of capacitor
for x in drange2(minX, maxX, xGap):
	for z in drange2(minZ, maxZ, zGap):
		addCharge(x, -topY, z, -unitCharge)

autogen(True)
genVectors()