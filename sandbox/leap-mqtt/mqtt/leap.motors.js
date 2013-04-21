
window.onkeypress = function(e) {
  if (e.charCode == 32) {
	 if (pausedFrame == null) {
		pausedFrame = latestFrame;
	 } else {
		pausedFrame = null;
	 }
  }
};


var controller = new Leap.Controller({enableGestures: true});
var myHand = null;
var handForwardBack = 0;
var handLeftRight = 0;
var pausedFrame = null;
var latestFrame = null;
var last = 0;
var updatePeriod = 100;
var xRangeCutoff = 0.25;
var phiRangeCutoff = Math.PI / 6;


controller.loop(function(frame) {
	latestFrame = frame;
	var html = "<div>Hand? " + (frame.hands.length > 0) + "</div>";

	if (!pausedFrame) {
		myHand = new HandData(frame, 0, 10);

		if (myHand.hand == null) {
			myHand = null;
		}

		var event = document.createEvent("Event");
		event.initEvent("tiltleap",true,true);
		event.forward = 0;
		event.yaw = 0;
		event.pitch = 0;

		if (myHand != null) {
			html += "<div>Direction: " + myHand.x + "," + myHand.y + "," + myHand.z + "</div>";
			html += "<div>Sampled: " + myHand.sampledX + "," + myHand.sampledY + "," + myHand.sampledZ + "</div>";
			html += "<div>Radius: " + myHand.radius + "</div>";
			html += "<div>SampledRadius: " + myHand.sampledRadius + "</div>";

			var r = Math.sqrt(myHand.x * myHand.x + myHand.y * myHand.y + myHand.z * myHand.z);
			//var theta = Math.acos(myHand.z / r);  // unused
			var coPhi = (Math.PI / 4) - Math.atan(myHand.y / myHand.x);

			if (myHand.x < -xRangeCutoff) {
				event.forward = 0;
				event.yaw = -100;
			} else if (myHand.x > xRangeCutoff) {
				event.forward = 0;
				event.yaw = 100;
			} else {
				event.forward = 100;
				event.yaw = 0;

				if (coPhi < -phiRangeCutoff) {
					event.pitch = -100;
				} else if (coPhi > phiRangeCutoff) {
					event.pitch = 100;
				}
			}
		}

		document.dispatchEvent(event);
	}

	var date = new Date();
	var now = date.getTime();

	if (now < last + updatePeriod) {
		return;
	}

	document.getElementById('out').innerHTML = html;
	last = now;
});

function HandData(frame, index, numSamples) {
	if (frame.hands.length <= index || !frame.hands[index].valid) {
		return null;
	}

	var hand = frame.hands[index];

	this.hand = hand;
	this.x = hand.direction[0];
	this.y = hand.direction[1];
	this.z = hand.direction[2];
	this.radius = hand.sphereRadius;

	var samples = 0;
	var x = 0;
	var y = 0;
	var z = 0;
	var rad = 0;

	for (var i = 1; i <= numSamples; i++) {
		var f = controller.frame(i);

		if (f.hands.length <= index || !f.hands[index].valid) {
			continue;
		}

		var oldHand = f.hands[index];

		samples++;
		x += oldHand.direction[0];
		y += oldHand.direction[1];
		z += oldHand.direction[2];
		rad += oldHand.sphereRadius;
	}

	if (samples > 0) {
		this.sampledX = x / samples;
		this.sampledY = y / samples;
		this.sampledZ = z / samples;
		this.sampledRadius = rad / samples;
	}
}
