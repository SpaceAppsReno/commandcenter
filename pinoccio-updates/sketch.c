
//
// Replace the getMotorPayload() function with the following code.
//
// Problems solved:
//  - Removes potential buffer overflow (but still keeps the buffer to the arbitrary 10 characters)
//  - Should be able to use this for more motors now; just keep calling parseMotorPayload() with
//    the current index and it will grab the next delimited value until the payload is empty
//
// I didn't have the header dependencies, so hopefully it compiles!
//

const int g_maxBuffer = 10;

static int parseMotorPayload(const byte* payload, const unsigned int length, unsigned int* index) {
  char buf[g_maxBuffer];
  int i = 0;

  while (*index < length && payload[*index] != ':' && i < g_maxBuffer) {  // leave room for the null terminator
    buf[i++] = payload[*index++];
  }

  buf[i] = '\0';
  return atoi(buf);
}

void getMotorPayload(byte* payload, unsigned int length) {
  int index = 0;

  if (!length) {
    return;
  }

  leftMotor = parseMotorPayload(payload, length, &index);
  rightMotor = parseMotorPayload(payload, length, &index);
}
