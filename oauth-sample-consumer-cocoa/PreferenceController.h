//
//  PreferenceController.h
//  oauth-sample-consumer
//
//  Created by Owen Evans on 20/02/2009.
//  Copyright 2009 Xero.com. All rights reserved.
//

#import <Cocoa/Cocoa.h>


@interface PreferenceController : NSWindowController {
	IBOutlet NSTextField *requestTokenUrl;
	IBOutlet NSTextField *requestAuthUrl;
	IBOutlet NSTextField *accessTokenUrl;
	IBOutlet NSTextField *consumerKey;
	IBOutlet NSTextField *consumerSecret;
}

-(IBAction) savePreferences:(id)sender;
-(IBAction) cancel:(id)sender;

@end
