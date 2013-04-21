var gesture = ('undefined' === typeof module ? {} : module.exports);
(function() {

    /**
     * cc.gesture
     * MIT Licensed
     */

    (function(exports, global) {

		// Store frame for motion functions
		var previousFrame = null;

		// Setup Leap loop with frame callback function
		var controllerOptions = {enableGestures: true};

		var handDirection;
		var handPalmNormal;
		var handPalmPosition;
		var handPalmVelocity;
		var handSphereCenter;
		var handSphereRadius;
		var rotationAxis;
		var rotationAngle;
		var translation;
		var scaleFactor;

		var leftMotorArray = new Array();
		var rightMotorArray = new Array();
		var motorArrayLength = 5;
		var leftMotor = 0;
		var rightMotor = 0;
		var betaSensitivity = 0.5;
		var gammaSensitivity = 0.3;


		var defaultHost = 'api.pinocc.io';
		var defaultControlTopic = 'erictj/3pi-control';
		var defaultTelemetryTopic = 'erictj/3pi-telemetry';

		var host;
		var controlTopic;
		var telemetryTopic;
		
		var socket;
    
 		 /**
         * gesture namespace.
         *
         * @namespace
         */

        var gesture = exports;

        /**
         * gesture version
         *
         * @api public
         */

        gesture.version = '0.0.1';
        

        /**
         * gesture armed - will send messages to device
         *
         * @api public
         */

        gesture.armed = false;
        
        /**
         * gesture online - connection established to socket server
         *
         * @api public
         */
		gesture.online = false;
         

        /**
         * Manages connections to hosts.
         *
         * @param {String} host - default 'api.pinocc.io'
         * @param {String} controlTopic - defualt 'erictj/3pi-control'
         * @param {String} telemetryTopic - default 'erictj/3pi-telemetry'
         * @api public
         */

		gesture.connect = function(host, controlTopic, telemetryTopic) {
			if(host == null || host == undefined) {
				host = defaultHost;
			}
			if(controlTopic == null || controlTopic == undefined) {
				controlTopic = defaultControlTopic;
			}
			if(telemetryTopic == null || telemetryTopic == undefined) {
				telemetryTopic = defaultTelemetryTopic;
			}
			socket = io.connect(host);
			socket.on('connect', function () {
				console.log("Connected to: " + host);
				gesture.online = true;
			});

			socket.on('disconnect', function () {
				console.log("Disconnected from: " + host);
				gesture.online = false;
			});
		};
		
		gesture.disconnect = function() {
			socket.disconnect();
		};

        /**
         * Checks for LEAP JavsScript client library
         *
         * @api public
         */
		gesture.checkLibrary = function() {
		  if (typeof Leap === "undefined") {
			alert("The Leap JavaScript client library (leap.js file) was not found. Please download the latest version from the GitHub project at https://github.com/leapmotion/leapjs");
			return false;
		  }
		  return true;
		};
		

		var tilt = function(forwardBack, leftRight) {
		//    console.log('tilt: ' + forwardBack + ' - leftRight: ' + leftRight);
			if (!gesture.armed) {
				return;
			}
			forwardBack = forwardBack * -1;
			var forward = (forwardBack * betaSensitivity).toFixed(0);
			var turn = (leftRight * gammaSensitivity).toFixed(0);

			leftMotorArray.push((+forward) + (+turn));
			if (leftMotorArray.length >= motorArrayLength) {
				leftMotorArray.shift();
			}
			rightMotorArray.push((+forward) + (+turn * -1));
			if (rightMotorArray.length >= motorArrayLength) {
				rightMotorArray.shift();
			}
		};

		setInterval(function() {
			if (!gesture.armed) {
				return;
			}
			
			for (var i=0; i < leftMotorArray.length; i++) {
				leftMotor += leftMotorArray[i];
			}
			for (var j=0; j < rightMotorArray.length; j++) {
				rightMotor += rightMotorArray[j];
			}

			leftMotor = (+leftMotor) / leftMotorArray.length;
			rightMotor = (+rightMotor) / rightMotorArray.length;

			leftMotor = +(leftMotor.toFixed());
			rightMotor = +(rightMotor.toFixed());

			var message = { 
				topic: controlTopic,
				message: leftMotor + ':' + rightMotor
			};
			socket.emit('publish', message);
			console.log('leftMotor: ' + leftMotor + ' - rightMotor: ' + rightMotor);
			
		}, 500);


		Leap.loop(controllerOptions, function(frame) {
			if (!gesture.armed) {
				return;
			}
		  
		  // Display Hand object data
		  if (frame.hands.length > 0) {
			for (var i = 0; i < frame.hands.length; i++) {
			  var hand = frame.hands[i];

			  handDirection = hand.direction;
			  handPalmNormal = hand.palmNormal;
			  handPalmPosition = hand.palmPosition;
			  handPalmVelocity = hand.palmVelocity;
			  handSphereCenter = hand.sphereCenter;
			  handSphereRadius = hand.sphereRadius;

			  // Hand motion factors
			  if (previousFrame) {
				translation = hand.translation(previousFrame);

				rotationAxis = hand.rotationAxis(previousFrame, 2);
				rotationAngle = hand.rotationAngle(previousFrame);

				scaleFactor = hand.scaleFactor(previousFrame);
			  }

			  var forwardBack = handPalmPosition[0];
			  var leftRight = handPalmPosition[2];
	  
			  if(forwardBack > 20) {
				forwardBack = 20;
			  }
			  else if(forwardBack < -20) {
				forwardBack = -20;
			  }
			  if(leftRight > 20) {
				leftRight = 20;
			  }
			  else if(forwardBack < -20) {
				leftRight = -20;
			  }
	  
			  tilt(forwardBack, leftRight);

			}
		  }
		  else {
			tilt(0, 0);
		  }

		  // Store frame for motion functions
		  previousFrame = frame;
		});
	})('object' === typeof module ? module.exports : (this.gesture = {}), this);
})();