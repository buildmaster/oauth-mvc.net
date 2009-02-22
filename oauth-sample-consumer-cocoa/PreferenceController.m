//
//  PreferenceController.m
//  oauth-sample-consumer
//
//  Created by Owen Evans on 20/02/2009.
//  Copyright 2009 Xero.com. All rights reserved.
//

#import "PreferenceController.h"


@implementation PreferenceController

-(id)init
{
	if(![super initWithWindowNibName:@"Preferences"])
	{
		return nil;
	}
	return self;
}

-(IBAction) cancel:(id)sender
{
	[self close];
}
-(IBAction) savePreferences:(id)sender
{
	
	
}
@end
