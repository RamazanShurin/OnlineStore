(function ($) {
    $.fn.myCarousel = function (options) {
        var settings = {
            visible: 3,
            rotateBy: 1,
            speed: 500,
            btnNext: null,
            btnPrev: null,
            auto: null,
            backSlide: false
        };

        return this.each(function () {
            if (options) {
                $.extend(settings, options);
            }

            var $this = $(this);
            var $carousel = $this.children(':first');
            var itemWidth = $carousel.children().outerWidth();
            var itemsTotal = $carousel.children().length;
            var running = false;
            var intID = null;

            $this.css({
                'position': 'relative',
                'overflow': 'hidden',
                'width': settings.visible * itemWidth + 'px'
            });

            $carousel.css({
                'position': 'relative',
                'width': 9999 + 'px',
                'left': 0
            });

            function slide(dir) {
                var direction = !dir ? -1 : 1;
                var leftIndent = 0;

                if (!running) {
                    running = true;

                    if (intID) {
                        window.clearInterval(intID);
                    }

                    if (!dir) {
                        $carousel.children(':last').after($carousel.children().slice(0, settings.rotateBy).clone(true));
                    } else {
                        $carousel.children(':first').before($carousel.children().slice(itemsTotal - settings.rotateBy, itemsTotal).clone(true));
                        $carousel.css('left', -itemWidth * settings.rotateBy + 'px');
                    }

                    leftIndent = parseInt($carousel.css('left')) + (itemWidth * settings.rotateBy * direction);

                    $carousel.animate({ 'left': leftIndent }, {
                        queue: false, duration: settings.speed, complete: function () {
                            if (!dir) {
                                $carousel.children().slice(0, settings.rotateBy).remove();
                                $carousel.css('left', 0);
                            } else {
                                $carousel.children().slice(itemsTotal, itemsTotal + settings.rotateBy).remove();
                            }

                            if (settings.auto) {
                                intID = window.setInterval(function () { slide(settings.backslide); }, settings.auto);
                            }

                            running = false;
                        }
                    });
                }

                return false;
            }

            $(settings.btnNext).click(function () {
                return slide(false);
            });

            $(settings.btnPrev).click(function () {
                return slide(true);
            });

            if (settings.auto) {
                intID = window.setInterval(function () { slide(settings.backslide); }, settings.auto);
            }
        });
    };
})(jQuery);