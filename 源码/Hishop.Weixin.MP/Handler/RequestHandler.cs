using Hishop.Weixin.MP;
using Hishop.Weixin.MP.Request;
using Hishop.Weixin.MP.Request.Event;
using Hishop.Weixin.MP.Util;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Linq;

namespace Hishop.Weixin.MP.Handler
{
	public abstract class RequestHandler
	{
		public XDocument RequestDocument
		{
			get;
			set;
		}

		public AbstractRequest RequestMessage
		{
			get;
			set;
		}

		public string ResponseDocument
		{
			get
			{
				string str;
				str = (ResponseMessage != null ? EntityHelper.ConvertEntityToXml<AbstractResponse>(ResponseMessage).ToString() : string.Empty);
				return str;
			}
		}

		public AbstractResponse ResponseMessage
		{
			get;
			set;
		}

		public RequestHandler(Stream inputStream)
		{
			XmlReader xmlReader = XmlReader.Create(inputStream);
			try
			{
                RequestDocument = XDocument.Load(xmlReader);
                Init(RequestDocument);
			}
			finally
			{
				if (xmlReader != null)
				{
					((IDisposable)xmlReader).Dispose();
				}
			}
		}

		public RequestHandler(string xml)
		{
			XmlReader xmlReader = XmlReader.Create(new StringReader(xml));
			try
			{
                RequestDocument = XDocument.Load(xmlReader);
                Init(RequestDocument);
			}
			finally
			{
				if (xmlReader != null)
				{
					((IDisposable)xmlReader).Dispose();
				}
			}
		}

		public abstract AbstractResponse DefaultResponse(AbstractRequest requestMessage);

		public void Execute()
		{
			if (RequestMessage != null)
			{
				switch (RequestMessage.MsgType)
				{
					case RequestMsgType.Text:
					{
                            ResponseMessage = OnTextRequest(RequestMessage as TextRequest);
						return;
					}
					case RequestMsgType.Image:
					{
                            ResponseMessage = OnImageRequest(RequestMessage as ImageRequest);
						return;
					}
					case RequestMsgType.Voice:
					{
                            ResponseMessage = OnVoiceRequest(RequestMessage as VoiceRequest);
						return;
					}
					case RequestMsgType.Video:
					{
                            ResponseMessage = OnVideoRequest(RequestMessage as VideoRequest);
						return;
					}
					case RequestMsgType.Location:
					{
                            ResponseMessage = OnLocationRequest(RequestMessage as LocationRequest);
						return;
					}
					case RequestMsgType.Link:
					{
                            ResponseMessage = OnLinkRequest(RequestMessage as LinkRequest);
						return;
					}
					case RequestMsgType.Event:
					{
                            ResponseMessage = OnEventRequest(RequestMessage as EventRequest);
						return;
					}
				}
				throw new WeixinException("未知的MsgType请求类型");
			}
		}

		private void Init(XDocument requestDocument)
		{
            RequestDocument = requestDocument;
            RequestMessage = RequestMessageFactory.GetRequestEntity(RequestDocument);
		}

		public virtual AbstractResponse OnEvent_ClickRequest(ClickEventRequest clickEventRequest)
		{
			return DefaultResponse(clickEventRequest);
		}

		public virtual AbstractResponse OnEvent_LocationRequest(LocationEventRequest locationEventRequest)
		{
			return DefaultResponse(locationEventRequest);
		}

		public virtual AbstractResponse OnEvent_ScanRequest(ScanEventRequest scanEventRequest)
		{
			return DefaultResponse(scanEventRequest);
		}

		public virtual AbstractResponse OnEvent_SubscribeRequest(SubscribeEventRequest subscribeEventRequest)
		{
			return DefaultResponse(subscribeEventRequest);
		}

		public virtual AbstractResponse OnEvent_UnSubscribeRequest(UnSubscribeEventRequest unSubscribeEventRequest)
		{
			return DefaultResponse(unSubscribeEventRequest);
		}

		public AbstractResponse OnEventRequest(EventRequest eventRequest)
		{
			AbstractResponse abstractResponse = null;
			switch (eventRequest.Event)
			{
				case RequestEventType.Subscribe:
				{
					abstractResponse = OnEvent_SubscribeRequest(eventRequest as SubscribeEventRequest);
					break;
				}
				case RequestEventType.UnSubscribe:
				{
					abstractResponse = OnEvent_UnSubscribeRequest(eventRequest as UnSubscribeEventRequest);
					break;
				}
				case RequestEventType.Scan:
				{
                        ResponseMessage = OnEvent_ScanRequest(eventRequest as ScanEventRequest);
					break;
				}
				case RequestEventType.Location:
				{
					abstractResponse = OnEvent_LocationRequest(eventRequest as LocationEventRequest);
					break;
				}
				case RequestEventType.Click:
				{
					abstractResponse = OnEvent_ClickRequest(eventRequest as ClickEventRequest);
					break;
				}
				default:
				{
					throw new WeixinException("未知的Event下属请求信息");
				}
			}
			return abstractResponse;
		}

		public virtual AbstractResponse OnImageRequest(ImageRequest imageRequest)
		{
			return DefaultResponse(imageRequest);
		}

		public AbstractResponse OnLinkRequest(LinkRequest linkRequest)
		{
			return DefaultResponse(linkRequest);
		}

		public virtual AbstractResponse OnLocationRequest(LocationRequest locationRequest)
		{
			return DefaultResponse(locationRequest);
		}

		public virtual AbstractResponse OnTextRequest(TextRequest textRequest)
		{
			return DefaultResponse(textRequest);
		}

		public virtual AbstractResponse OnVideoRequest(VideoRequest videoRequest)
		{
			return DefaultResponse(videoRequest);
		}

		public virtual AbstractResponse OnVoiceRequest(VoiceRequest voiceRequest)
		{
			return DefaultResponse(voiceRequest);
		}
	}
}