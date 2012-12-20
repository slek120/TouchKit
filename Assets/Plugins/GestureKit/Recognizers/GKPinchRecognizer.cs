using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class GKPinchRecognizer : GKAbstractGestureRecognizer
{
	public float deltaScale = 0;
	private float _intialDistance;
	private float _previousDistance;
	
	
	private float distanceBetweenTrackedTouches()
	{
		return Vector2.Distance( _trackingTouches[0].position, _trackingTouches[1].position );
	}
	
	
	public override void touchesBegan( List<GKTouch> touches )
	{
		if( state == GKGestureRecognizerState.Possible )
		{
			// we need to have two touches to work with so we dont set state to Begin until then
			// latch the touches
			for( int i = 0; i < touches.Count; i++ )
			{
				// only add touches in the Began phase
				if( touches[i].phase == TouchPhase.Began )
				{
					_trackingTouches.Add( touches[i] );
					
					if( _trackingTouches.Count == 2 )
						break;
				}
			}
			
			if( _trackingTouches.Count == 2 )
			{
				deltaScale = 0;
				_intialDistance = distanceBetweenTrackedTouches();
				_previousDistance = _intialDistance;
				state = GKGestureRecognizerState.RecognizedAndStillRecognizing;
			}
		}
	}
	
	
	public override void touchesMoved( List<GKTouch> touches )
	{
		if( state == GKGestureRecognizerState.RecognizedAndStillRecognizing )
		{
			var currentDistance = distanceBetweenTrackedTouches();
			deltaScale = ( currentDistance - _previousDistance ) / _intialDistance;
			_previousDistance = currentDistance;
			state = GKGestureRecognizerState.RecognizedAndStillRecognizing;
		}
	}
	
	
	public override void touchesEnded( List<GKTouch> touches )
	{
		// remove any completed touches
		for( int i = 0; i < touches.Count; i++ )
		{
			if( touches[i].phase == TouchPhase.Ended )
				_trackingTouches.Remove( touches[i] );
		}
		
		// if we still have a touch left continue to wait for another. no touches means its time to reset
		if( _trackingTouches.Count == 1 )
		{
			state = GKGestureRecognizerState.Possible;
			deltaScale = 1;
		}
		else
		{
			state = GKGestureRecognizerState.Failed;
		}
	}
	
	
	public override string ToString()
	{
		return string.Format( "[{0}] state: {1}, deltaScale: {2}", this.GetType(), state, deltaScale );
	}

}
