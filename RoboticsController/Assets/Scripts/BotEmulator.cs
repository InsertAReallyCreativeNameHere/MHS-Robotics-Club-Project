using UnityEngine;
using System.Threading;
using System.Threading.Tasks;

public class BotEmulator : MonoBehaviour
{
    public HingeJoint[] wheels;
    public ConfigurableJoint[] pivots;
    
    public enum Wheels : short
    {
        UL,
        UR,
        LL,
        LR
    }

    public enum Polarity : short
    {
        Positive = 1,
        Negative = -1
    }

    public async Task StartMotorAsync(Polarity polarity)
    {
        for (int i = 0; i < wheels.Length; i++)
        {
            JointMotor motor = wheels[i].motor;
            motor.force = 10000;
            motor.targetVelocity = 340 * (int)polarity;
            wheels[i].motor = motor;
        }
    }

    public async Task StopMotorAsync(bool brake)
    {
        for (int i = 0; i < wheels.Length; i++)
        {
            JointMotor motor = wheels[i].motor;
            if (brake)
            {
                motor.force = 10000;
            }
            else
            {
                motor.force = 0;
            }
            motor.targetVelocity = 0;
            wheels[i].motor = motor;
        }
    }

    public async Task TurnLeft_UNDEF() //This has not been implemented on the real bot.
    {
        pivots[(int)Wheels.UL].targetRotation = Quaternion.Euler(-15, 0, 0);
        pivots[(int)Wheels.UR].targetRotation = Quaternion.Euler(-15, 0, 0);
        pivots[(int)Wheels.LL].targetRotation = Quaternion.Euler(15, 0, 0);
        pivots[(int)Wheels.LR].targetRotation = Quaternion.Euler(15, 0, 0);
    }

    public async Task TurnRight_UNDEF() //This has not been implemented on the real bot.
    {
        pivots[(int)Wheels.UL].targetRotation = Quaternion.Euler(15, 0, 0);
        pivots[(int)Wheels.UR].targetRotation = Quaternion.Euler(15, 0, 0);
        pivots[(int)Wheels.LL].targetRotation = Quaternion.Euler(-15, 0, 0);
        pivots[(int)Wheels.LR].targetRotation = Quaternion.Euler(-15, 0, 0);
    }

    public async Task ResetTurn_UNDEF() //This has not been implemented on the real bot.
    {
        pivots[(int)Wheels.UL].targetRotation = Quaternion.Euler(0, 0, 0);
        pivots[(int)Wheels.UR].targetRotation = Quaternion.Euler(0, 0, 0);
        pivots[(int)Wheels.LL].targetRotation = Quaternion.Euler(0, 0, 0);
        pivots[(int)Wheels.LR].targetRotation = Quaternion.Euler(0, 0, 0);
    }
}
