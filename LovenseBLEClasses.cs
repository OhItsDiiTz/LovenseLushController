using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LovenseBLE
{
    class LovenseBLEClasses
    {

    }


    public class ToySupport
    {
        public string type;
        public string showName;
        public LovenseCommandType[] supported;
    }
    public enum LovenseCommandType
    {
        VIBRATE = 0,
        VIBRATE1 = 1,
        VIBRATE2 = 2,
        VIBRATE3 = 3,
        ROTATE = 4,
        PUMP = 5,
        THRUSTRING = 6,
        FINGERING = 7,
        SUCTION = 8,
        DEPTH = 9,
        POSITION = 10,
        OSCILLATE = 11,
        GET_BATTERY = 12,
    };
    public enum LovenseDataReportingEventType
    {
        KEY_DOWN = 0,
        KEY_UP = 1,
        VIBTATE = 2,
        ROTATE = 3,
        SHAKE = 4,
        MOVEMENT = 5,
        DEEP = 6,
        AIR_IN = 7,
        AIR_OUT = 8,
        AIR_CLOSE = 9,
        AIR_OPEN = 10,
        BATTERY = 11,
        STROKE_SPEED_POSITION = 12,
        SHAKE_TIMES = 13,
    };

    public enum InitSupportCommandStatus
    {
        SUCCESS,
        FAILED
    }

    public enum CheckBLEResultStatus
    {
        BLE_ENABLE_OPEN = 0,
        BLE_ENABLE_CLOSE,
        BLE_DISABLE,
        EXCEPTION
    }

    public enum LovenseNotifyType
    {
        SCAN_START = 0,
        SCAN_STOP = 1,
        EXECUTE_SUCCESS = 2,
        EXECUTE_FAILED = 3,
        FAILED_SCANNINGING_NOW = 4,
        DIS_CONNECTION_SUCCESS = 5,
        DIS_CONNECTION_ERROR = 6,
        CONNECTION_SUCCESS = 7,
        CONNECTION_ERROR = 8,
        TOY_NO_CONNECT = 9,
        BLE_DISABLE_OR_CLOSE = 10,
    }

    public class LovenseCommand
    {
        public LovenseCommandType commandType;
        public int value;
    }

    public class LovenseSolaceProCommand : LovenseCommand
    {
        public int speedLevel;
        public int strokeLow;
        public int strokeHigh;
    }

    public class LovenseToy
    {
        public string id;
        public string name;
        public string type;
        public string mac;
        public bool connected;

        public override bool Equals(object obj)

        {

            return Equals(obj as LovenseToy);

        }

        public bool Equals(LovenseToy other)

        {

            if (other is null)

                return false;

            if (ReferenceEquals(this, other))

                return true;

            return id == other.id;
        }

        public override int GetHashCode()

        {
            return id.GetHashCode();
        }
    }
}
