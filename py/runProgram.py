import jkzuc
c = jkzuc.command()
s = jkzuc.stat()

if __name__ == '__main__':
    c.teleop_enable(0)
    c.wait_complete()
    s.poll()
    for jnum in range(0, 6):
        if not (s.homed[jnum]):
            c.home(-1)
            c.wait_complete()
            break
    # open the file
    c.task_plan_synch()
    c.wait_complete()
    filePath = '/home/jakauser/massage_rel.ngc'
    c.program_open(filePath)
    # ensure the mode
    s.poll()
    if s.task_mode != jkzuc.MODE_AUTO:
        c.mode(jkzuc.MODE_AUTO)  # task_mode
        c.wait_complete()
        s.poll()
    c.auto(jkzuc.AUTO_RUN, 0)

