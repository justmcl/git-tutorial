#!/usr/bin/env python
# -*- coding:utf-8 -*-

import socket
import traceback
import time
import json
import csv
import math
import datetime

open_file_name = "jaka_test1.csv"  # 写入需要读取的数据文件名
IP = '192.168.1.101'  # 机器人的IP

def test_servo_j():
    tcp_server_addr = (IP, 10001)
    sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

    try:#连接机器人
        sock.connect(tcp_server_addr)
        print('Socket connect success.')

    except Exception():
        print(traceback.format_exc())
    #与相机通讯
    listenSocket = socket.socket()         # 创建 socket 对象
    host = socket.gethostname() # 获取本地主机名
    print(host)
    port = 12345                # 设置端口
    listenSocket.bind((host, port))        # 绑定端口

    listenSocket.listen(10)
    print ('Waitting for connect...')
    dataSocket,addr = listenSocket.accept()     # 建立客户端连接
    print ('连接成功，地址：', addr)
    while True:
        recData=dataSocket.recv(51200)
        Data=recData.decode()
        dic = json.loads(Data) #得到字典
        if dic['type']=='areuok?':
            dataSocket.send('yes'.encode())
            print ('准备接收数据...')
            while True:
                recData=dataSocket.recv(51200)
                Data=recData.decode()
                dic = json.loads(Data) #得到字典
                print(f'收到信息{Data}')
                for point in dic['points']:
                    print(0)
                    carPosition = [ point[0],point[1], point[2], point[3], point[4],point[5]]
                    # carPosition = [float dic['points'][0],float dic['points'][1],float dic['points'][2],float dic['points'][3],float dic['points'][4],float dic['points'][5]]

                    testDict0 = '{"cmdName":"joint_move","jointPosition":[0,90,0,90,180,0],"speed":20,"relFlag":0}'
                    testDict  = '{"cmdName":"moveL","cartPosition":[ 0,0,0,0,0,0],"speed":100.0, "relFlag":0}'
                    print(1)

                    data = json.loads(testDict)
                    data["cartPosition"] = carPosition
                    testDict = json.dumps(data)
                    sock.sendall(testDict.encode())
                    print(2)
                    recvdata = sock.recv(2048).decode()
                    print("senddata to server:", testDict)
                    print("recvdata in client:", recvdata)


    #dataSocket.send(f'服务端接收到了信息: {Data}'.encode())
    dataSocket.close()
    lisenSocket.close()
    # 解析文件
    
   
    print("play end!")
    #sock.close()

test_servo_j()
