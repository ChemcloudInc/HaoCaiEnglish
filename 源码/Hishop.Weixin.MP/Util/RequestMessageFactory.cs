using Hishop.Weixin.MP;
using Hishop.Weixin.MP.Request;
using Hishop.Weixin.MP.Request.Event;
using System;
using System.Xml.Linq;

namespace Hishop.Weixin.MP.Util
{
	public static class RequestMessageFactory
	{
		public static AbstractRequest GetRequestEntity(XDocument doc)
		{
			RequestMsgType msgType = MsgTypeHelper.GetMsgType(doc);
			AbstractRequest textRequest = null;
			switch (msgType)
			{
				case RequestMsgType.Text:
				{
					textRequest = new TextRequest();
					break;
				}
				case RequestMsgType.Image:
				{
					textRequest = new ImageRequest();
					break;
				}
				case RequestMsgType.Voice:
				{
					textRequest = new VoiceRequest();
					break;
				}
				case RequestMsgType.Video:
				{
					textRequest = new VideoRequest();
					break;
				}
				case RequestMsgType.Location:
				{
					textRequest = new LocationRequest();
					break;
				}
				case RequestMsgType.Link:
				{
					textRequest = new LinkRequest();
					break;
				}
				case RequestMsgType.Event:
				{
					switch (EventTypeHelper.GetEventType(doc))
					{
						case RequestEventType.Subscribe:
						{
							textRequest = new SubscribeEventRequest();
							break;
						}
						case RequestEventType.UnSubscribe:
						{
							textRequest = new UnSubscribeEventRequest();
							break;
						}
						case RequestEventType.Scan:
						{
							textRequest = new ScanEventRequest();
							break;
						}
						case RequestEventType.Location:
						{
							textRequest = new LocationEventRequest();
							break;
						}
						case RequestEventType.Click:
						{
							textRequest = new ClickEventRequest();
							break;
						}
						default:
						{
							throw new ArgumentOutOfRangeException();
						}
					}
					break;
				}
				default:
				{
					throw new ArgumentOutOfRangeException();
				}
			}
			EntityHelper.FillEntityWithXml<AbstractRequest>(textRequest, doc);
			return textRequest;
			throw new ArgumentOutOfRangeException();
		}
	}
}