import socket               # 导入 socket 模块
import json
 
dataSocket = socket.socket()         # 创建 socket 对象
host = socket.gethostname() # 获取本地主机名
port = 10001                # 设置端口号
js0='{"cmdName":"set_compliant_type","sensor_compensation":1,"compliance_type":1}'
js1='{"cmdName":"get_compliant_type"}'
jssend=""
dataSocket.connect(("192.168.1.101", port))

while True:
    toSend=input('??')
    if toSend.rstrip()=="s0" :
        dict0=json.loads(js0)
        dict0["compliance_type"]=0
        jssend=json.dumps(dict0)
    elif toSend.rstrip()=="s10" :
        dict0=json.loads(js0)
        dict0["compliance_type"]=1
        jssend=json.dumps(dict0)
    elif toSend.rstrip()=="s20" :
        dict0=json.loads(js0)
        dict0["compliance_type"]=2
        jssend=json.dumps(js0)
    elif toSend.rstrip()=="g" :
        jssend=js1

<<<<<<< HEAD
    dataSocket.send(jssend.encode())
=======
    dataSocket.send(jssend.encode())
>>>>>>> e2e19c235fee68f922002862de87d7caf42b74e0
    received = dataSocket.recv(512000)
    revstr = received.decode()
    print(revstr)

dataSocket.close()
