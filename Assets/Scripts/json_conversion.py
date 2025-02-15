import json


with open('C:/Users/jmr/Documents/GitHub/satellite-pose-sim/Assets/test_sl.json') as f:
    data = json.load(f)

poses = []
for pose in data:
    poses.append(pose)

dict = {'poses':poses}



with open('C:/Users/jmr/Documents/GitHub/satellite-pose-sim/Assets/sl.json', 'w') as json_file:
  json.dump(dict, json_file, indent = 2,separators=(',', ': '))

