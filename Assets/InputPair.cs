using System.Collections;
using System.Collections.Generic;

public class InputPair
{
    public IEnumerator input;
    public float timestamp;

    public InputPair(IEnumerator i, float t) {
        input = i;
        timestamp = t;
    }
}
