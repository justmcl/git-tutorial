# Copyright 2015-2019 - RoboDK Inc. - https://robodk.com/
# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
# http://www.apache.org/licenses/LICENSE-2.0
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.

# ----------------------------------------------------
# This file is a POST PROCESSOR for Robot Offline Programming to generate programs 
# for a Universal Robot with RoboDK
#
# To edit/test this POST PROCESSOR script file:
# Select "Program"->"Add/Edit Post Processor", then select your post or create a new one.
# You can edit this file using any text editor or Python editor. Using a Python editor allows to quickly evaluate a sample program at the end of this file.
# Python should be automatically installed with RoboDK
#
# You can also edit the POST PROCESSOR manually:
#    1- Open the *.py file with Python IDLE (right click -> Edit with IDLE)
#    2- Make the necessary changes
#    3- Run the file to open Python Shell: Run -> Run module (F5 by default)
#    4- The "test_post()" function is called automatically
# Alternatively, you can edit this file using a text editor and run it with Python
#
# To use a POST PROCESSOR file you must place the *.py file in "C:/RoboDK/Posts/"
# To select one POST PROCESSOR for your robot in RoboDK you must follow these steps:
#    1- Open the robot panel (double click a robot)
#    2- Select "Parameters"
#    3- Select "Unlock advanced options"
#    4- Select your post as the file name in the "Robot brand" box
#
# To delete an existing POST PROCESSOR script, simply delete this file (.py file)
#
# ----------------------------------------------------
# More information about RoboDK Post Processors and Offline Programming here:
#     https://robodk.com/help#PostProcessor
#     https://robodk.com/doc/en/PythonAPI/postprocessor.html
# ----------------------------------------------------

DEFAULT_HEADER_SCRIPT = """
  // this generated script is suitale for jkzuc 1.4.x
"""

# SCRIPT_URP = '''<URProgram name="%s">
#  <children>
#    <MainProgram runOnlyOnce="false" motionType="MoveJ" speed="1.0471975511965976" acceleration="1.3962634015954636" useActiveTCP="false">
#      <children>
#        <Script type="File">
#          <cachedContents>%s
# </cachedContents>
#          <file resolves-to="file">%s</file>
#        </Script>
#      </children>
#    </MainProgram>
#  </children>
# </URProgram>'''

# SCRIPT_URP = '''<URProgram createdIn="3.0.0" lastSavedIn="3.0.0" name="%s" directory="/" installation="default">
#  <children>
#    <MainProgram runOnlyOnce="true" motionType="MoveJ" speed="1.0471975511965976" acceleration="1.3962634015954636" useActiveTCP="false">
#      <children>
#        <Script type="File">
#          <cachedContents>%s
# </cachedContents>
#          <file resolves-to="file">%s</file>
#        </Script>
#      </children>
#    </MainProgram>
#  </children>
# </URProgram>'''

# <URProgram createdIn="3.4.3.361" lastSavedIn="3.4.3.361" name="%s" directory="." installation="default">
SCRIPT_URP = '''<URProgram createdIn="3.0.0" lastSavedIn="3.0.0" name="%s" directory="." installation="default">
  <children>
    <MainProgram runOnlyOnce="true" motionType="MoveJ" speed="1.0471975511965976" acceleration="1.3962634015954636" useActiveTCP="false">
      <children>
        <Script type="File">
          <cachedContents>%s
</cachedContents>
          <file>%s</file>
        </Script>
      </children>
    </MainProgram>
  </children>
</URProgram>'''


def get_safe_name(progname):
    """Get a safe program name"""
    for c in r'-[]/\;,><&*:%=+@!#^|?^':
        progname = progname.replace(c, '')
    if len(progname) <= 0:
        progname = 'Program'
    if progname[0].isdigit():
        progname = 'P' + progname
    return progname


# ----------------------------------------------------
# Import RoboDK tools
from robodk import *

# ----------------------------------------------------
import socket
import struct
import ftplib
import traceback
import os
import JAKAZu
import hashlib

# UR information for real time control and monitoring
# Byte shifts for the real time packet:
UR_GET_RUNTIME_MODE = 132 * 8 - 4

RUNTIME_CANCELLED = 0
RUNTIME_READY = 1
RUNTIME_BUSY = 2

RUNTIME_MODE_MSG = []
RUNTIME_MODE_MSG.append("Operation cancelled")  # 0
RUNTIME_MODE_MSG.append("Ready")  # 1
RUNTIME_MODE_MSG.append("Running")  # 2 # Running or Jogging


# Get packet size according to the byte array
def UR_packet_size(buf):
    if len(buf) < 4:
        return 0
    return struct.unpack_from("!i", buf, 0)[0]


# Check if a packet is complete
def UR_packet_check(buf):
    msg_sz = UR_packet_size(buf)
    if len(buf) < msg_sz:
        print("Incorrect packet size %i vs %i" % (msg_sz, len(buf)))
        return False

    return True


# Get specific information from a packet
def UR_packet_value(buf, offset, nval=6):
    if len(buf) < offset + nval:
        print("Not available offset (maybe older Polyscope version?): %i - %i" % (len(buf), offset))
        return None
    format = '!'
    for i in range(nval):
        format += 'd'
    return list(struct.unpack_from(format, buf, offset))  # return list(struct.unpack_from("!dddddd", buf, offset))


ROBOT_PROGRAM_ERROR = -1
ROBOT_NOT_CONNECTED = 0
ROBOT_OK = 1


def GetErrorMsg(rec_bytes):
    idx_error = -1
    try:
        idx_error = rec_bytes.index(b'error')
    except:
        return None

    if idx_error >= 0:
        idx_error_end = min(idx_error + 20, len(rec_bytes))
        try:
            idx_error_end = rec_bytes.index(b'\0', idx_error)
        except:
            return "Unknown error"
    return rec_bytes[idx_error:idx_error_end].decode("utf-8")


def pose_2_str(pose):
    """Prints a pose target"""
    [x, y, z, w, p, r] = Pose_2_Motoman(pose)
    return ('%.6f, %.6f, %.6f, %.6f, %.6f, %.6f' % (x, y, z, w, p, r))


def angles_2_str(angles):
    """Prints a joint target"""
    njoints = len(angles)
    # d2r = pi/180.0
    d2r = 1  # ur uses radian but jaka uses degree so the ratio is 1
    if njoints == 6:
        return ('%.6f, %.6f, %.6f, %.6f, %.6f, %.6f' % (
            angles[0] * d2r, angles[1] * d2r, angles[2] * d2r, angles[3] * d2r, angles[4] * d2r, angles[5] * d2r))
    else:
        return 'this post only supports 6 joints'


def circle_radius(p0, p1, p2):
    a = norm(subs3(p0, p1))
    b = norm(subs3(p1, p2))
    c = norm(subs3(p2, p0))
    radius = a * b * c / sqrt(pow(a * a + b * b + c * c, 2) - 2 * (pow(a, 4) + pow(b, 4) + pow(c, 4)))
    return radius


# def distance_p1_p02(p0,p1,p2):
#    v01 = subs3(p1, p0)
#    v02 = subs3(p2, p0)
#    return dot(v02,v01)/dot(v02,v02)

# ----------------------------------------------------    
# Object class that handles the robot instructions/syntax
class RobotPost(object):
    """Robot post object for JAKA RoboScripts before version2.0
    derived from Universal_Robots.py """

    MAX_LINES_X_PROG = 1e9  # 250    # Maximum number of lines per program. If the number of lines is exceeded, the program will be executed step by step by RoboDK
    PROG_EXT = '.ngc'  # set the program extension
    ZU_EXT = '.zus'  # set the Zu file extension
    SPEED_MMS = 25.0  # default speed for linear moves in mm/s
    SPEED_DEGS_PROG = 33.333  # default speed for joint moves in deg/s add prog for distinguish
    ACCEL_MMSS = 16.666  # default acceleration for lineaer moves in mm/ss
    ACCEL_DEGSS = 11.111  # default acceleration for joint moves in rad/s

    ToolMaxAcc = 50.0
    ToolMaxSpeed = 10
    JointMaxSpeed = 1.8
    JointMaxAcc = 7.2

    BLEND_RADIUS_MM = 1.0  # default blend radius in meters (corners smoothing)
    MOVEC_MIN_RADIUS = 1  # minimum circle radius to output (in mm). It does not take into account the Blend radius
    MOVEC_MAX_RADIUS = 10000  # maximum circle radius to output (in mm). It does not take into account the Blend radius
    USE_MOVEP = False
    # --------------------------------
    REF_FRAME = eye(4)  # default reference frame (the robot reference frame)
    LAST_POS_ABS = None  # last XYZ position

    # other variables
    ROBOT_POST = 'JAKA_beforeV14'
    ROBOT_NAME = 'JAKA'
    PROG_FILES = []
    MAIN_PROGNAME = 'unknown'

    nPROGS = 0
    PROG = []
    PROG_LIST = []
    VARS = []
    VARS_LIST = []
    SUBPROG = []
    TAB = ''
    LOG = ''
    LINECOMMENT = '//'
    Prog = JAKAZu.JAKAZuProg()
    ZuJson = ''
    ZuFilePath = ''
    VariablePath = ''
    MainZuFilePath = ''

    def __init__(self, robotpost=None, robotname=None, robot_axes=6, **kwargs):
        self.ROBOT_POST = robotpost
        self.ROBOT_NAME = robotname
        for k, v in kwargs.items():
            if k == 'lines_x_prog':
                self.MAX_LINES_X_PROG = v

    def ProgStart(self, progname):
        progname = get_safe_name(progname)
        self.nPROGS = self.nPROGS + 1
        if self.nPROGS <= 1:
            self.TAB = ''
            # Create global variables:
            self.vars_update()
            self.MAIN_PROGNAME = progname
        else:
            pass

    def ProgFinish(self, progname):
        progname = get_safe_name(progname)
        self.TAB = ''
        if self.nPROGS <= 1:
            self.addline('// End of main program')
        else:
            raise Exception('Only one prog is supported.')

    def ProgSave(self, folder, progname, ask_user=False, show_result=False):
        progname = get_safe_name(progname)
        zuname = progname + self.ZU_EXT #生成.zus文件
        progname = progname + self.PROG_EXT
        mainProgName = "RoboDK.ngc"
        mainzuname = "RoboDK.zu"
        if ask_user or not DirExists(folder):
            filesave = getSaveFile(folder, mainProgName, 'Save program as...')
            if filesave is not None:
                filesave = filesave.name
                fpath, fname = os.path.split(filesave)
                zufilepath = fpath + '/' + zuname
                variablepath = fpath + '/variable.vi' #生成.vi文件
                mainzufilePath = fpath + '/' + mainzuname
            else:
                return
        else:
            filesave = folder + '/' + mainProgName
            zufilepath = folder + '/' + zuname
            variablepath = folder + '/variable.vi'
            mainzufilePath = folder + '/' + mainzuname

        self.prog_2_list()
        fid = open(filesave, "w")
        # Create main program call:
        # fid.write('def %s():\n' % self.MAIN_PROGNAME)

        # Add a custom header if desired:
        fid.write(DEFAULT_HEADER_SCRIPT)
        fid.write('  \n')

        # Add global parameters:
        fid.write('  //Global parameters:\n')
        for line in self.VARS_LIST[0]:
            fid.write(line + '\n')
            # fid.write('  \n')
        fid.write('  ')
        # Add the suprograms that are being used in RoboDK

        # Add the main code:
        fid.write('// Main program:\n')
        for prog in self.PROG_LIST:
            for line in prog:
                fid.write(line + '\n')

        fid.write('M2\n\n')
        # fid.write('%s()\n' % self.MAIN_PROGNAME)

        fid.close()

        print('SAVED: %s\n' % filesave)  # tell RoboDK the path of the saved file
        self.PROG_FILES = filesave

        mainProg = JAKAZu.JAKAZuProg()
        mainZuJson = mainProg.tojsons()
        mainZuW = open(mainzufilePath, "w")
        mainZuW.write(mainZuJson)
        mainZuW.close()
        self.MainZuFilePath = mainzufilePath

        self.ZuJson = self.Prog.tojsons()
        # m1 = hashlib.md5()
        # m1.update(self.ZuJson.encode('utf-8'))
        # md5str = m1.hexdigest()
        # print("md5str="+md5str)
        # self.ZuJson = self.ZuJson.replace('"md5"', '"{0}"'.format(md5str))
        # print("replace=", self.ZuJson)
        zuW = open(zufilepath, "w")
        zuW.write(self.ZuJson)
        zuW.close()
        self.ZuFilePath = zufilepath

        varW = open(variablepath, "w")
        varW.write('{"serializableVariableItemLst":[],"Count":0}')
        varW.close()
        self.VariablePath = variablepath

        # open file with default application
        if show_result:
            if type(show_result) is str:
                # Open file with provided application
                import subprocess
                p = subprocess.Popen([show_result, filesave])
            elif type(show_result) is list:
                import subprocess
                p = subprocess.Popen(show_result + [filesave])
            else:
                # open file with default application
                os.startfile(filesave)
            if len(self.LOG) > 0:
                mbox('Program generation LOG:\n\n' + self.LOG)

        # if len(self.PROG_LIST) > 1:
        #    mbox("Warning! The program " + progname + " is too long and directly running it on the robot controller might be slow. It is better to run it form RoboDK.")

    def ProgSendRobot(self, robot_ip, remote_path, ftp_user, ftp_pass):
        """Send a program to the robot using the provided parameters. This method is executed right after ProgSave if we selected the option "Send Program to Robot".
        The connection parameters must be provided in the robot connection menu of RoboDK"""
        ftp = ftplib.FTP()
        try:
            ftp.connect(robot_ip, 2121)
            ftp.login(user='jaka', passwd='jaka12345')
            ftp.cwd('program')
            # self.PROG_FILES="C:/testdir/robodk2.ngc"
            fullname = self.PROG_FILES
            zufullname = self.ZuFilePath
            variablefullName = self.VariablePath
            mainzufullname = self.MainZuFilePath
            basename = os.path.basename(fullname)
            zuname = os.path.basename(zufullname)
            variablename = os.path.basename(variablefullName)
            mainzuname = os.path.basename(mainzufullname)
            (filename, ext) = os.path.splitext(basename)
            try:
                ftp.cwd(filename)
                self.filelist(ftp)
                self.count()
                if not self.IsExits(basename):
                    ftp.storbinary('STOR ' + basename, open(fullname, 'rb'))
                if not self.IsExits(mainzuname):
                    ftp.storbinary('STOR ' + mainzuname, open(mainzufullname, 'rb'))
                if not self.IsExits(variablename):
                    ftp.storbinary('STOR ' + variablename, open(variablefullName, 'rb'))
                try:
                    ftp.cwd("sub")
                    ftp.storbinary('STOR ' + zuname, open(zufullname, 'rb'))
                except Exception:
                    ftp.mkd("sub")
                    ftp.cwd("sub")
                    ftp.storbinary('STOR ' + zuname, open(zufullname, 'rb'))
            except Exception:
                try:
                    ftp.mkd(filename)
                    ftp.cwd(filename)
                    ftp.storbinary('STOR ' + basename, open(fullname, 'rb'))
                    ftp.storbinary('STOR ' + mainzuname, open(mainzufullname, 'rb'))
                    ftp.storbinary('STOR ' + variablename, open(variablefullName, 'rb'))
                    ftp.mkd("sub")
                    ftp.cwd("sub")
                    ftp.storbinary('STOR ' + zuname, open(zufullname, 'rb'))
                except Exception:
                    print("Mkdir failed %s", filename)
                    print(traceback.format_exc())
                    import pdb
                    pdb.set_trace()
            ftp.quit()

        except:
            print(traceback.format_exc())
        finally:
            sys.stdout.flush()

        # UploadFTP(self.PROG_FILES, robot_ip, remote_path, ftp_user, ftp_pass)
        return

    dir_sum = 0
    res_sum = 0
    size_sum = 0
    filePaths = []

    def filelist(self, ftp):
        self.filePaths = []
        for name, facts in ftp.mlsd(".", ["type", "size"]):
            if facts["type"] == "dir":
                if ftp.pwd().endswith('/'):
                    dir_cwd = ftp.pwd() + name
                else:
                    dir_cwd = ftp.pwd() + os.sep + name
                try:
                    self.dir_sum += 1
                    self.filelist(dir_cwd)
                    ftp.cwd('..')
                except:
                    pass
            else:
                self.res_sum += 1
                self.size_sum += int(facts["size"])
                if ftp.pwd().endswith('/'):
                    res_path = ftp.pwd() + name
                else:
                    res_path = ftp.pwd() + os.sep + name
                # print(res_path)
                res_name = os.path.basename(res_path)
                self.filePaths.append(res_name)

    def count(self):
        count_msg = f"文件夹: {self.dir_sum}, 文件数： {self.res_sum}, 总大小: {self.size_sum / 1024 / 1024}/MB"
        print(count_msg)

    def IsExits(self, name):
        for path in self.filePaths:
            if path == name:
                return True
        return False

    def blend_radius_check(self, pose_abs, ratio_check=0.4):
        # check that the blend radius covers 40% of the move (at most)
        blend_radius = self.BLEND_RADIUS_MM
        # return blend_radius
        current_pos = pose_abs.Pos()
        if self.LAST_POS_ABS is None:
            blend_radius = '0'
        else:
            distance = norm(subs3(self.LAST_POS_ABS, current_pos))  # in mm
            if ratio_check * distance < self.BLEND_RADIUS_MM:
                blend_radius = '%.3f' % (round(ratio_check * distance, 3))
        # self.LAST_POS_ABS = current_pos
        return blend_radius

    def MoveJ(self, pose, joints, conf_RLF=None):
        """Add a joint movement"""
        target = '0,0,0,0,0,0'
        if pose is None:
            blend_radius = "0"
            self.LAST_POS_ABS = None
        else:
            pose_abs = self.REF_FRAME * pose
            blend_radius = self.blend_radius_check(pose_abs)
            target = pose_2_str(pose_abs)
            self.LAST_POS_ABS = pose_abs.Pos()

        if len(joints) < 6:
            raise Exception('Cannot generate MoveJ without 6 joints.')
        else:
            # self.addline('#<endPosJ>={%s}' % angles_2_str(joints))
            # self.addline('MOVJ(#<endPosJ>,0,#<speed_degs>,#<accel_degss>,%s)' % blend_radius)
            MJ = JAKAZu.ZuMoveJ(target, joints, self.SPEED_DEGS_PROG, self.ACCEL_DEGSS, 2, blend_radius)
            self.Prog.append_ins(MJ)

    def MoveL(self, pose, joints, conf_RLF=None):
        """Add a linear movement"""
        # Movement in joint space or Cartesian space should give the same result:
        # pose_wrt_base = self.REF_FRAME*pose
        # self.addline('movel(%s,accel_mss,speed_ms,0,blend_radius_m)' % (pose_2_str(pose_wrt_base)))
        target = '0,0,0,0,0,0'
        if pose is None:
            blend_radius = "0"
            self.LAST_POS_ABS = None
        else:
            pose_abs = self.REF_FRAME * pose
            blend_radius = self.blend_radius_check(pose_abs)
            target = pose_2_str(pose_abs)
            self.LAST_POS_ABS = pose_abs.Pos()

        if len(joints) < 6:
            raise Exception('Cannot generate MoveJ without 6 joints.')
        else:
            # self.addline('#<endPosJ>={%s}' % angles_2_str(joints))
            # self.addline('MOVJ(#<endPosJ>,0,#<speed_degs>,#<accel_degss>,%s)' % blend_radius)
            MJ = JAKAZu.ZuMoveJ(target, joints, self.SPEED_DEGS_PROG, self.ACCEL_DEGSS, 2, blend_radius)
            self.Prog.append_ins(MJ)
        
            """if pose is None:
            raise Exception('Cannot generate MoveL without pose.')
        else:
            #pose_abs = self.REF_FRAME * pose
            pose_abs = pose
            blend_radius = self.blend_radius_check(pose_abs)
            target = pose_2_str(pose_abs)
            self.LAST_POS_ABS = pose_abs.Pos()

        # self.addline('#<endPosL>={%s}' % target)
        # self.addline('MOVL(#<endPosL>,0,#<speed_mms>,#<accel_mmss>,%s)' % blend_radius)
        ML = JAKAZu.ZuMoveL(target, joints, self.SPEED_MMS, self.ACCEL_MMSS, 2, blend_radius)
        self.Prog.append_ins(ML)"""
    def MoveC(self, pose1, joints1, pose2, joints2, conf_RLF_1=None, conf_RLF_2=None):
        """Add a circular movement"""
        pose1_abs = self.REF_FRAME * pose1
        pose2_abs = self.REF_FRAME * pose2
        p0 = self.LAST_POS_ABS
        p1 = pose1_abs.Pos()
        p2 = pose2_abs.Pos()
        if p0 is None:
            self.MoveL(pose2, joints2, conf_RLF_2)
            return

        radius = circle_radius(p0, p1, p2)
        print("MoveC Radius: " + str(radius) + " mm")
        if radius < self.MOVEC_MIN_RADIUS or radius > self.MOVEC_MAX_RADIUS:
            self.MoveL(pose2, joints2, conf_RLF_2)
            return

        blend_radius = self.blend_radius_check(pose1_abs, 0.2)
        # blend_radius = '%.3f' % (0.001*radius) #'0'
        # blend_radius = '0'
        self.LAST_POS_ABS = pose2_abs.Pos()
        # self.addline('#<varMidPOSL>={%s}' % pose_2_str(pose1_abs))
        # self.addline('#<varEndPOSL>={%s}' % pose_2_str(pose2_abs))
        # self.addline('MOVC(#<varMidPOSL>,#<varEndPOSL>,0,#<speed_mms>,#<accel_mmss>,%s)' % blend_radius)
        CL = JAKAZu.ZuMoveC(pose_2_str(pose1_abs), joints1, pose_2_str(pose2_abs), joints2, self.SPEED_MMS,
                            self.ACCEL_MMSS, 2, blend_radius)
        self.Prog.append_ins(CL)

    def setFrame(self, pose, frame_id=None, frame_name=None):
        """Change the robot reference frame"""
        # the reference frame is not needed if we use joint space for joint and linear movements
        # the reference frame is also not needed if we use cartesian moves with respect to the robot base frame
        # the cartesian targets must be pre-multiplied by the active reference frame
        self.REF_FRAME = pose
        self.addline('#<usrframe>={%s}' % pose_2_str(pose))
        self.addline('SETUSERFRAME(#<usrframe>)')
        ucs = JAKAZu.ZuUcs(pose_2_str(pose))
        self.Prog.append_ins(ucs)

    def setTool(self, pose, tool_id=None, tool_name=None):
        """Change the robot TCP"""
        self.addline('#<tcpframe>={%s}' % pose_2_str(pose))
        self.addline('SETTOOL(#<tcpframe>)')
        tcp = JAKAZu.ZuTcp(pose_2_str(pose))
        self.Prog.append_ins(tcp)
        # self.addline('set_payload(1.4, [-0.1181, -0.1181, 0.03])')
        # self.addline('set_gravity([0.0, 0.0, 9.82]))')
        # if 'Tool 1' in tool_name:
        # self.addline('set_payload(1.4, [-0.1181, -0.1181, 0.03])')

    def Pause(self, time_ms):
        """Pause the robot program"""
        pass

    def setSpeed(self, speed_mms):
        """Changes the robot speed (in mm/s)"""
        # if speed_mms < 999.9:
        #    self.USE_MOVEP = True
        # else:
        #    self.USE_MOVEP = False
        if speed_mms > 100:
            speed_mms = 100
        self.SPEED_MMS = speed_mms
        self.addline('#<speed_mms> = %.3f' % float(self.SPEED_MMS * self.ToolMaxSpeed))

    def setAcceleration(self, accel_mmss):
        """Changes the robot acceleration (in mm/s2)"""
        if accel_mmss > 100:
            accel_mmss = 100
        self.ACCEL_MMSS = accel_mmss
        self.addline('#<accel_mmss>   = %.3f' % float(self.ACCEL_MMSS * self.ToolMaxAcc))

    def setSpeedJoints(self, speed_degs):
        """Changes the robot joint speed (in deg/s)"""
        if speed_degs > 100:
            speed_degs = 100
        self.SPEED_DEGS_PROG = speed_degs
        self.addline('#<speed_degs>  = %.3f' % float(speed_degs * self.JointMaxSpeed))

    def setAccelerationJoints(self, accel_degss):
        """Changes the robot joint acceleration (in deg/s2)"""
        if accel_degss > 100:
            accel_degss = 100
        self.ACCEL_DEGSS = accel_degss
        self.addline('#<accel_degss> = %.3f' % float(self.ACCEL_DEGSS * self.JointMaxAcc))

    def setZoneData(self, zone_mm):
        """Changes the zone data approach (makes the movement more smooth)"""
        if zone_mm < 0:
            zone_mm = 0
        self.BLEND_RADIUS_MM = zone_mm
        self.addline('#<blend_radius_mm> = %.3f' % self.BLEND_RADIUS_MM)

    def setDO(self, io_var, io_value):
        """Set a Digital Output"""
        self.addline('%s Set a Digital Output io_var=%s,io_value=%s' % (self.LINECOMMENT, io_var, io_value))

    def setAO(self, io_var, io_value):
        """Set an Analog Output"""
        self.addline('%s Set an Analog Output io_var=%s,io_value=%s' % (self.LINECOMMENT, io_var, io_value))

    def waitDI(self, io_var, io_value, timeout_ms=-1):
        """Waits for an input io_var to attain a given value io_value. Optionally, a timeout can be provided."""
        pass

    def RunCode(self, code, is_function_call=False):
        """Adds code or a function call"""
        pass
        # self.addline('# ' + code) #generate custom code as a comment

    def RunMessage(self, message, iscomment=False):
        """Show a message on the controller screen"""
        if iscomment:
            self.addline(self.LINECOMMENT + message)
        else:
            pass

    # ------------------ private ----------------------
    def vars_update(self):
        # Generate global variables for this program
        self.VARS = []
        self.VARS.append('#<speed_mms>    = %.3f' % float(self.SPEED_MMS * self.ToolMaxSpeed))
        self.VARS.append('#<speed_degs>  = %.3f' % float(self.SPEED_DEGS_PROG * self.JointMaxSpeed))
        self.VARS.append('#<accel_mmss>   = %.3f' % float(self.ACCEL_MMSS * self.ToolMaxAcc))
        self.VARS.append('#<accel_degss> = %.3f' % float(self.ACCEL_DEGSS * self.JointMaxAcc))
        self.VARS.append('#<blend_radius_mm> = %.3f' % self.BLEND_RADIUS_MM)

    def prog_2_list(self):
        if len(self.PROG) > 1:
            self.PROG_LIST.append(self.PROG)
            self.PROG = []
            self.VARS_LIST.append(self.VARS)
            self.VARS = []
            self.vars_update()

    def addline(self, newline):
        """Add a program line"""
        if self.nPROGS <= 1:
            if len(self.PROG) > self.MAX_LINES_X_PROG:
                self.prog_2_list()

            self.PROG.append(newline)
        else:
            self.SUBPROG.append(newline)

    def addlog(self, newline):
        """Add a log message"""
        self.LOG = self.LOG + newline + '\n'


# -------------------------------------------------
# ------------ For testing purposes ---------------   
def Pose(xyzrpw):
    [x, y, z, r, p, w] = xyzrpw
    a = r * math.pi / 180
    b = p * math.pi / 180
    c = w * math.pi / 180
    ca = math.cos(a)
    sa = math.sin(a)
    cb = math.cos(b)
    sb = math.sin(b)
    cc = math.cos(c)
    sc = math.sin(c)
    return Mat([[cb * ca, ca * sc * sb - cc * sa, sc * sa + cc * ca * sb, x],
                [cb * sa, cc * ca + sc * sb * sa, cc * sb * sa - ca * sc, y], [-sb, cb * sc, cc * cb, z], [0, 0, 0, 1]])


def test_post():
    """Test the post with a basic program"""

    robot = RobotPost('JAKA', 'Generic JAKA robot')
    robot.ProgStart("Program")
    robot.RunMessage("Program generated by RoboDK", True)
    robot.setFrame(Pose([807.766544, -963.699898, 41.478944, 0, 0, 0]))
    robot.setTool(Pose([62.5, -108.253175, 100, -60, 90, 0]))
    robot.setSpeed(100)  # set speed to 100 mm/s
    robot.setAcceleration(3000)  # set speed to 3000 mm/ss
    robot.setZoneData(9.9)
    robot.MoveJ(Pose([200, 200, 500, 180, 0, 180]), [-46.18419, -6.77518, -20.54925, 71.38674, 49.58727, -302.54752])
    robot.MoveJ(Pose([200, 200, 500, 180, 0, 180]), [-46.18419, -6.77518, -20.54925, 71.38674, 49.58727, -302.54752])
    robot.MoveJ(Pose([200, 200, 500, 180, 0, 180]), [-46.18419, -6.77518, -20.54925, 71.38674, 49.58727, -302.54752])
    robot.MoveJ(Pose([200, 200, 500, 180, 0, 180]), [-46.18419, -6.77518, -20.54925, 71.38674, 49.58727, -302.54752])
    robot.MoveL(Pose([200, 250, 348.734575, 180, 0, -150]),
                [-41.62707, -8.89064, -30.01809, 60.62329, 49.66749, -258.98418])
    robot.MoveL(Pose([200, 200, 262.132034, 180, 0, -150]),
                [-43.73892, -3.91728, -35.77935, 58.57566, 54.11615, -253.81122])
    robot.RunMessage("Setting air valve 1 on")
    robot.RunCode("TCP_On", True)
    robot.Pause(1000)
    robot.MoveL(Pose([200, 250, 348.734575, 180, 0, -150]),
                [-41.62707, -8.89064, -30.01809, 60.62329, 49.66749, -258.98418])
    robot.MoveL(Pose([250, 300, 278.023897, 180, 0, -150]),
                [-37.52588, -6.32628, -34.59693, 53.52525, 49.24426, -251.44677])
    robot.MoveL(Pose([250, 250, 191.421356, 180, 0, -150]),
                [-39.75778, -1.04537, -40.37883, 52.09118, 54.15317, -246.94403])
    robot.RunMessage("Setting air valve off")
    robot.RunCode("TCP_Off", True)
    robot.Pause(1000)
    robot.MoveL(Pose([250, 300, 278.023897, 180, 0, -150]),
                [-37.52588, -6.32628, -34.59693, 53.52525, 49.24426, -251.44677])
    robot.MoveL(Pose([250, 200, 278.023897, 180, 0, -150]),
                [-41.85389, -1.95619, -34.89154, 57.43912, 52.34162, -253.73403])
    robot.MoveL(Pose([250, 150, 191.421356, 180, 0, -150]),
                [-43.82111, 3.29703, -40.29493, 56.02402, 56.61169, -249.23532])
    # robot.MoveJ(Pose([425.457, 96.141, 33.050, 90, 0, 11.261]), [0, 0, 0, 0, 0, 0])
    robot.MoveC(Pose([-580.925, 112.012, 368.150, 90.000, 0, -90.000]),
                [56.000000, 85.452400, 94.729700, 180.817900, 146.000000, 0.000000],
                Pose([-580.925, 112.012, 207.213, 90, 0, -90.000]),
                [0.000000, 122.282473, 78.016486, 159.701041, 90.000000, -0.000000])
    robot.ProgFinish("Program")
    robot.ProgSave(".", "Program", True)
    robot.ProgSendRobot("192.168.2.71", "/", "", "")
    for line in robot.PROG:
        print(line)
    if len(robot.LOG) > 0:
        mbox('Program generation LOG:\n\n' + robot.LOG)

    input("Press Enter to close...")


if __name__ == "__main__":
    """Function to call when the module is executed by itself: test"""
    test_post()
