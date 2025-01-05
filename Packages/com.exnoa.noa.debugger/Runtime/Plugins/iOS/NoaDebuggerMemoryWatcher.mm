#import <mach/mach.h>
 
extern "C"
{
    bool GetTaskVmInfo(task_vm_info_data_t *info)
    {
        mach_msg_type_number_t count = TASK_VM_INFO_COUNT;
        if(task_info(mach_task_self(), TASK_VM_INFO, (task_info_t)info, &count) != KERN_SUCCESS)
        {
            return false;
        }
        return true;
    }
 
    long NoaDebuggerGetCurrentMemoryByte()
    {
        task_vm_info_data_t info;
        if(!GetTaskVmInfo(&info))
        {
            return -1;
        }
        return (long)(info.phys_footprint);
    }
}
