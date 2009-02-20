//
//  ApplicationController.h
//  oauth-sample-consumer
//
//  Created by Owen Evans on 20/02/2009.
//  Copyright 2009 Xero.com. All rights reserved.
//

#import <Cocoa/Cocoa.h>
#import "PreferenceController.h"

@interface ApplicationController : NSObject {
	PreferenceController *preferenceController;

}
-(IBAction) showPreferenceController:(id)sender;
@end
