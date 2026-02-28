"Section Slicer" - Custom Feature
    Anthony Lu
    July 2022
    
    Slices a solid body into sections of uniform thickness with rectangular slots for fitting together. The orientation of sliced sections may be adjusted by selecting a coordinate system in the settings, and uses world coordinates by default. The slicer arranges its sections along their respective slicer axes, with its X-axis serving as the reference axis. In two-axis mode, the skew angle of the U-axis (angle with respect to the slicer Y-axis) is adjustable.
    In three-axis mode, the angles between X, U, V axes are fixed to produce a hexagonal pattern between their sections, and restrictions are enforced to avoid regions where sections of all three axes intersect. The U, V axes are fixed to skew angles of 30 deg and -30 deg, respectively. The section space must be greater than twice the section width to give V-axis sections enough clearance.
    The resulting sections are named and numbered according to their axis.
    The convention is to prefix position and direction vectors with lowercase letters 'w' for world coordinates, and 'l' for local (slicer) coordinates.
