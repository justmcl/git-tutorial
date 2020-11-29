# -*- coding: utf-8 -*-
"""
Created on Thu Apr  2 10:02:31 2020

@author: linmianhao
"""

 
import sys
import time
print(sys.path)
#COORD_BASE = 0    # 基坐标系
#COORD_JOINT = 1   # 关节空间
#COORD_TOOL = 2    #工具坐标系



import jkrc
robot = jkrc.RC("192.168.2.64")#返回一个机器人对象,此IP需要替换成自己使用的机器人IP
robot.login()  #登录
ret = robot.get_joint_position()
if ret[0] == 0:
    print("the joint position is :",ret[1])
else:
    print("some things happend,the errcode is: ",ret[0])          
robot.logout()  #登出    
