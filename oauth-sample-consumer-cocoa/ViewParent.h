//
//  ViewParent.h
//  oauth-sample-consumer
//
//  Created by Owen Evans on 20/02/2009.
//  Copyright 2009 Xero.com. All rights reserved.
//


@protocol ViewParent<NSObject>
	-(void) setViewName:(NSString *) parent;
-(id) getSharedValue:(NSString*) key;
-(void) setSharedValue:(id)value 
				forKey:(NSString*) key;
@end
