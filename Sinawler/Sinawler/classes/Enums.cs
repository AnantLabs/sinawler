using System;
using System.Collections.Generic;
using System.Text;

namespace Sinawler
{
    //Every Enums used in the program
    public enum DataFormat { JSON = 0, XML = 1 };
    public enum QueueBufferFor { USER_RELATION = 0, USER_INFO = 1, USER_TAG = 2, STATUS = 3, COMMENT = 4 };
    public enum SysArgFor { USER_RELATION = 0, USER_INFO = 1, USER_TAG = 2, STATUS = 3, COMMENT = 4 };
    public enum RelationState { RelationExists = 1, RelationCanceled = 0 };
    public enum UserState { UserExists = 1, UserNotExists = 0 };
}
