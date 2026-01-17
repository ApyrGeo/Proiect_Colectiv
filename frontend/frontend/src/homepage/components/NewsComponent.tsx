import React, { useMemo } from "react";
import { Box, Card, CardContent, CardMedia, Typography, Container } from "@mui/material";
import { useTranslation } from "react-i18next";
import { Swiper, SwiperSlide } from "swiper/react";
import { Navigation, Pagination, EffectCoverflow } from "swiper/modules";
import newsDataEn from "./newsData.en.json";
import newsDataRo from "./newsData.ro.json";
import newsDataHu from "./newsData.hu.json";
import newsDataDe from "./newsData.de.json";
import "./NewsComponent.css";

export interface NewsItem {
  id: number;
  title: string;
  description: string;
  image?: string;
  date?: string;
}

interface NewsComponentProps {
  items?: NewsItem[];
  title?: string;
}

const NEWS_DATA_MAP: Record<string, typeof newsDataEn> = {
  en: newsDataEn,
  ro: newsDataRo,
  hu: newsDataHu,
  de: newsDataDe,
};

const NewsComponent: React.FC<NewsComponentProps> = ({ items, title }) => {
  const { t, i18n } = useTranslation();

  const displayItems = useMemo(() => {
    let baseItems: NewsItem[] = [];

    if (items && items.length > 0) {
      baseItems = items;
    } else {
      const newsData = NEWS_DATA_MAP[i18n.language] || newsDataEn;
      baseItems = newsData.newsItems;
    }

    return [...baseItems, ...baseItems, ...baseItems];
  }, [items, i18n.language]);

  return (
    <Box className="news-component">
      <Container maxWidth="lg">
        <h1>{title || t("news.title")}</h1>

        <Swiper
          modules={[Navigation, Pagination, EffectCoverflow]}
          effect="coverflow"
          grabCursor={true}
          centeredSlides={true}
          loop={true}
          loopAddBlankSlides={false}
          slidesPerView="auto"
          spaceBetween={20}
          preventInteractionOnTransition={true}
          coverflowEffect={{
            rotate: 40,
            stretch: 0,
            depth: 150,
            modifier: 1,
            slideShadows: false,
          }}
          pagination={{
            el: ".swiper-pagination",
            clickable: true,
            enabled: false,
          }}
          navigation={{
            nextEl: ".swiper-button-next",
            prevEl: ".swiper-button-prev",
          }}
          className="news-swiper"
        >
          {displayItems.map((item) => (
            <SwiperSlide key={item.id} className="news-swiper-slide">
              <Card className="news-card">
                {item.image && (
                  <CardMedia
                    component="img"
                    height="250"
                    image={item.image}
                    alt={item.title}
                    className="news-card-image"
                  />
                )}
                <CardContent className="news-card-content">
                  <Typography variant="h6" className="news-card-title" gutterBottom>
                    {item.title}
                  </Typography>
                  <Typography variant="body2" className="news-card-description" gutterBottom>
                    {item.description}
                  </Typography>
                  {item.date && (
                    <Typography variant="caption" className="news-card-date">
                      {item.date}
                    </Typography>
                  )}
                </CardContent>
              </Card>
            </SwiperSlide>
          ))}

          <div className="swiper-button-prev"></div>
          <div className="swiper-button-next"></div>
          <div className="swiper-pagination"></div>
        </Swiper>
      </Container>
    </Box>
  );
};

export default NewsComponent;
