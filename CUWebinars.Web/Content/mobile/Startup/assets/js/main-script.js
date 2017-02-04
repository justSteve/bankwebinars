(function ($) {
    "use strict";

    $(window).load(function(){

        $('#loading').fadeOut(1500);
        $('#into-logo').addClass('animated fadeInDown');
        $('#into-btn').addClass('animated zoomIn');

    });

    $(document).ready(function() {

        /* Scrolling Smoothly
         * ----------------------------------------------------------------------------------------*/
        $('a[href*=#]:not([href=#])').on('click', function() {
            if (location.pathname.replace(/^\//,'') == this.pathname.replace(/^\//,'')
                || location.hostname == this.hostname) {

                var target = $(this.hash);
                target = target.length ? target : $('[name=' + this.hash.slice(1) +']');
                if (target.length) {
                    $('html,body').animate({
                        scrollTop: target.offset().top - 0
                    }, 1000);
                    return false;
                }
            }
        });

        /*SIDE NAV
         * ----------------------------------------------------------------------------------------*/
        $("#nav-btn").on('click', function() {
            $("#side-nav").animate({right: 0}, 'medium');
            $("#side-nav-mask").addClass('visible');
        });
        $("#side-nav-mask").on('click', function() {
            $("#side-nav").animate({right: -250}, 'medium');
            $("#side-nav-mask").removeClass('visible');
        });

        /*Parallax
         * ----------------------------------------------------------------------------------------*/
        $(window).stellar({
            horizontalScrolling: false
        });

        /*Type Effect
         * ----------------------------------------------------------------------------------------*/
        var typist;
        typist = document.querySelector("#typist-element");
        new Typist(typist, {
            letterInterval: 60,
            textInterval: 5000
        });

        /*SERVICES
         * ----------------------------------------------------------------------------------------*/
        
        $(".single-service").hover(function(){
            $('.ic', this)
                .removeClass('animated rubberBand')
                .hide()
                .addClass('animated rubberBand')
                .show();
        });

        /*Products
         * ----------------------------------------------------------------------------------------*/
        $(".single-product").hover(function(){
            $('.product-overlay', this)
                .addClass('animated fadeIn visible')
                .show();
        });

        $(".single-product").mouseleave(function(){
            $('.product-overlay', this)
                .removeClass('animated fadeOut visible')
                .hide()

        });

        /*Swiper
         * ----------------------------------------------------------------------------------------*/
        var teamSwiper = new Swiper ('#team-slider', {
            slidesPerView: 1,
            loop: true,
            direction: 'horizontal',
            effect: "slide",
            speed: 1000,
            autoplay: 5000,
            spaceBetween: 0,
            pagination: '.swiper-pagination',
            paginationClickable: true,
            autoplayDisableOnInteraction: true,
        });

    });

})(jQuery);
