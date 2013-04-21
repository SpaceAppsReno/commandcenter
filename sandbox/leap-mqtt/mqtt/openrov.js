var mq = new Mosquitto();
  //var url = 'ws://api.pinocc.io';
  var url = 'ws://test.mosquitto.org/ws';
  var controlTopic = 'erictj/3pi-control';
  var telemetryTopic = 'erictj/3pi-telemetry';
  var armed = false;

  $(function() {
    var online = false;
    var leftMotorArray = new Array();
    var rightMotorArray = new Array();
    var upMotorArray = new Array();
    var motorArrayLength = 5;
    var leftMotor = 0;
    var rightMotor = 0;
    var upMotor = 0;
    var betaSensitivity = 0.5;
    var gammaSensitivity = 0.3;
    var zetaSensitivity = 0.2;

    // mqtt connection and message-recieve handling
    mq.connect(url, true);
    mq.subscribe(telemetryTopic, 0);

    mq.onmessage = function(topic, payload, qos) {
      if (payload == 'Still alive') {
        if ($('#alive-ping').html() == '&amp;nbsp;') {
          if (!online) {
            online = true;
            $('#rover-messages-container').html('Pinoccio 3pi online');
          }
          $('#alive-ping').html('&amp;bull;');
        } else {
          $('#alive-ping').html('&amp;nbsp;');
        }
      } else {
        $('#rover-messages-container').append(payload);
      }
    };

    // device accelerometer handling
    if (window.DeviceOrientationEvent) {
      window.addEventListener('deviceorientation', function() {
        tilt(event.beta, event.gamma, 0);
      }, true);
    }

    document.addEventListener("tiltleap", function() {
      tilt(-event.forward, event.yaw, event.pitch);
    }, true);

    var tilt = function(forwardBack, leftRight, upDown) {
      if (!armed) {
        return;
      }
      forwardBack = forwardBack * -1;
      var forward = (forwardBack * betaSensitivity).toFixed(0);
      var turn = (leftRight * gammaSensitivity).toFixed(0);
      var up = (upDown * zetaSensitivity).toFixed(0);

      $('#forward-amount').html(forward);
      $('#turn-amount').html(turn);

      leftMotorArray.push((+forward) + (+turn));
      if (leftMotorArray.length >= motorArrayLength) {
        leftMotorArray.shift();
      }
      rightMotorArray.push((+forward) + (+turn * -1));
      if (rightMotorArray.length >= motorArrayLength) {
        rightMotorArray.shift();
      }
      upMotorArray.push((+up));
      if (upMotorArray.length >= motorArrayLength) {
        upMotorArray.shift();
      }
    }

    // interface-button handling
    $('#rov-go').click(function(e) {
      if (!armed) {
        armed = true;
        $('#rov-go').html("Driving enabled!");
      } else {
        armed = false;
        $('#rov-go').html("Enable driving");
        $('#forward-amount').html('-');
        $('#turn-amount').html('-');
      }
    });

    // only send control messages twice a second (2Hz), and use smoothing via averaging
    setInterval(function() {
      if (!armed) {
        return;
      }

      for (var i=0; i < leftMotorArray.length; i++) {
        leftMotor += leftMotorArray[i];
      }
      for (var j=0; j < rightMotorArray.length; j++) {
        rightMotor += rightMotorArray[j];
      }
      for (var j=0; j < upMotorArray.length; j++) {
        upMotor += upMotorArray[j];
      }

      leftMotor = (+leftMotor) / leftMotorArray.length;
      rightMotor = (+rightMotor) / rightMotorArray.length;
      upMotor = (+upMotor) / upMotorArray.length;

      leftMotor = +(leftMotor.toFixed());
      rightMotor = +(rightMotor.toFixed());
      upMotor = +(upMotor.toFixed());

      //mq.publish(controlTopic, leftMotor + ':' + rightMotor, 0);
      console.log('leftMotor: ' + leftMotor + ' - rightMotor: ' + rightMotor + ' - upMotor: ' + upMotor);
    }, 500);
  });